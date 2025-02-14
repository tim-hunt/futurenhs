﻿using System.Globalization;
using System.Net;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Caching.Memory;

namespace FutureNHS.Api.DataAccess.Storage.Providers
{
    public sealed class BlobStorageProvider : IFileBlobStorageProvider, IImageBlobStorageProvider
    {
        const int TOKEN_SAS_TIMEOUT_IN_MINUTES = 40;                 // Aligns with authentication cookie timeout policy for which we have an NFR

        private const string BlobStorageDownloadUserDelegationKeyCacheKey =
            "AzureFileBlobStorageDownload:UserDelegationKey";

        private readonly Uri _downloadEndpoint;
        private readonly ILogger<BlobStorageProvider> _logger;
        private readonly ISystemClock _systemClock;

        private readonly CloudBlobContainer? _cloudBlobContainer;


        public BlobStorageProvider(ISystemClock systemClock, string connectionString, string containerName, Uri downloadEndpoint, ILogger<BlobStorageProvider> logger, IMemoryCache memoryCache)
        {
            _systemClock = systemClock;
            _logger = logger;
            _downloadEndpoint = downloadEndpoint;

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            _cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
        }

        public async Task<string?> UploadFileAsync(Stream stream, string blobName, string contentType, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));

                cancellationToken.ThrowIfCancellationRequested();

                if (_cloudBlobContainer is null)
                {
                    throw new InvalidOperationException("A connection to blob storage was not opened");
                }

                var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

                blob.Properties.ContentType = contentType;
                await blob.UploadFromStreamAsync(stream, cancellationToken);
                return blob.Properties.ContentMD5;
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Blob Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                throw;
            }

            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed:'");

                throw;
            }
        }

        public async Task<string?> UploadFileAsync(byte[] bytes, string blobName, string contentType, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));

                cancellationToken.ThrowIfCancellationRequested();

                if (_cloudBlobContainer is null)
                {
                    throw new InvalidOperationException("A connection to blob storage was not opened");
                }

                var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

                blob.Properties.ContentType = contentType;
                await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, cancellationToken);
                return blob.Properties.ContentMD5;
            }
            catch (AuthenticationFailedException ex)
            {
                _logger?.LogError(ex, "Unable to authenticate with the Azure Blob Storage service using the default credentials.  Please ensure the user account this application is running under has permissions to access the Blob Storage account we are targeting");

                throw;
            }
            catch (RequestFailedException ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed: '{StatusCode} {StatusCodeName}'", ex.Status, Enum.Parse(typeof(HttpStatusCode), Convert.ToString(ex.Status, CultureInfo.InvariantCulture)));

                throw;
            }

            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unable to access the storage endpoint as the download request failed:'");

                throw;
            }
        }

        public async Task DeleteFileAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));


            if (_cloudBlobContainer is null)
            {
                throw new InvalidOperationException("A connection to blob storage was not opened");
            }

            // use a CloudBlockBlob because both BlobBlockClient and BlobClient buffer into memory for uploads
            var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

            await blob.DeleteIfExistsAsync();
        }

        public string GetRelativeDownloadUrl(string blobName, string fileName, SharedAccessBlobPermissions downloadPermissions, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));
            if (_cloudBlobContainer == null) throw new InvalidOperationException("A connection to blob storage was not created"); ;

            var blob = _cloudBlobContainer.GetBlockBlobReference(blobName);

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = downloadPermissions,
                SharedAccessStartTime = _systemClock.UtcNow,
                SharedAccessExpiryTime = _systemClock.UtcNow.AddMinutes(TOKEN_SAS_TIMEOUT_IN_MINUTES)
            };

            var sasBlobHeaders = new SharedAccessBlobHeaders()
            {
                // allows us to download file with original filename not blob name
                ContentDisposition = $"attachment; filename=\"{fileName}\""
            };

            try
            {
                var sas = blob.GetSharedAccessSignature(policy, sasBlobHeaders);

                return $"{_downloadEndpoint}/{blobName}{sas}";
            }

            //TODO catch specific exception for failing to generate sas token
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Unable to generate download token for blob: {blobName}'", blobName);
                throw new ApplicationException("Unable to generate download token");
            }
        }
    }
}
