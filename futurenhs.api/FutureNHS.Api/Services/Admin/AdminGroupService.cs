﻿using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Helpers.Interfaces;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Validation;
using Ganss.XSS;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Net;
using System.Security;
using System.Text;

namespace FutureNHS.Api.Services.Admin
{
    public sealed class AdminGroupService : IAdminGroupService
    {
        private const string AddGroupRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/add";

        private readonly ILogger<AdminGroupService> _logger;
        private readonly IImageBlobStorageProvider _blobStorageProvider;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;
        private readonly IFileTypeValidator _fileTypeValidator;
        private readonly IGroupCommand _groupCommand;
        private readonly IGroupImageService _imageService;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IContentService _contentService;

        // Notification template Ids
        private readonly string[] _acceptedFileTypes = new[] { ".png", ".jpg", ".jpeg" };
        private const long MaxFileSizeBytes = 5242880; // 5MB

        public AdminGroupService(ISystemClock systemClock,
            ILogger<AdminGroupService> logger,
            IPermissionsService permissionsService,
            IImageBlobStorageProvider blobStorageProvider,
            IFileTypeValidator fileTypeValidator,
            IGroupImageService imageService,
            IGroupCommand groupCommand,
            IHtmlSanitizer htmlSanitizer,
            IContentService contentService)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _blobStorageProvider = blobStorageProvider ?? throw new ArgumentNullException(nameof(blobStorageProvider));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _fileTypeValidator = fileTypeValidator ?? throw new ArgumentNullException(nameof(fileTypeValidator));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
        }

        public async Task CreateGroupAsync(Guid userId, Stream requestBody, string? contentType, CancellationToken cancellationToken)
        {
            var now = _systemClock.UtcNow.UtcDateTime;

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AddGroupRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: CreateGroupAsync - User:{0} does not have access to add group:{1}", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            var (group, image) = await AdminUploadGroupImageMultipartContent(userId, requestBody, contentType, cancellationToken);

            if (image is not null)
            {
                try
                {
                    var imageId = await _imageService.CreateImageAsync(image);
                    group = group with { ImageId = imageId };
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: CreateImageAsync - Error creating image {0}", null);
                    if (image is not null)
                    {
                        await _blobStorageProvider.DeleteFileAsync(image.FileName);
                        await _imageService.DeleteImageAsync(image.Id);
                    }
                    throw new DBConcurrencyException("Error: User request to create an image was not successful");
                }
            }

            var groupValidator = new GroupValidator();
            var groupValidationResult = await groupValidator.ValidateAsync(group, cancellationToken);
            if (groupValidationResult.Errors.Count > 0)
                throw new ValidationException(groupValidationResult);

            var groupSiteDto = new GroupSiteDto()
            {
                CreatedAtUTC = now,
                CreatedBy = userId,
            };

            try
            {
                groupSiteDto.GroupId = await _groupCommand.CreateGroupAsync(userId, group, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                if (image is not null)
                {
                    await _blobStorageProvider.DeleteFileAsync(image.FileName);
                    await _imageService.DeleteImageAsync(image.Id);
                }
                _logger.LogError(ex, $"Error: CreateGroupAsync - Error creating group {0}", null);
                throw new DBConcurrencyException("Error: User request to create a group was not successful");
            }

            // Create the group homepage content in Umbraco
            var createContentResponse = await _contentService.CreatePageAsync(userId, groupSiteDto.GroupId, null, cancellationToken);

            // If the create content request fails, delete the associated group
            if (createContentResponse is null || !createContentResponse.Succeeded)
            {
                //TODO: rollback/delete group (feature: 75323)
                _logger.LogError($"Error: CreatePageAsync - Error creating group homepage content {groupSiteDto.GroupId}");
                throw new HttpRequestException($"Error: CreatePageAsync - Error creating group homepage content {groupSiteDto.GroupId}");
            }

            try
            {
                groupSiteDto.ContentRootId = Guid.Parse(createContentResponse.Data);
                await _groupCommand.CreateGroupSiteAsync(groupSiteDto, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: CreateGroupSiteAsync - Error creating group site {0}");
                throw new DBConcurrencyException("Error: User request to create a group site was not successful");
            }
        }


        private async Task<(GroupDto, ImageDto?)> AdminUploadGroupImageMultipartContent(Guid userId, Stream requestBody, string? contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if HttpRequest (Form Data) is a Multipart Content Type
            if (!MultipartRequestHelper.IsMultipartContentType(contentType))
            {
                throw new InvalidDataException($"Expected a multipart request, but got {contentType}");
            }

            var now = _systemClock.UtcNow.UtcDateTime;

            var defaultFormOptions = new FormOptions();
            // Create a Collection of KeyValue Pairs.
            var formAccumulator = new KeyValueAccumulator();
            // Determine the Multipart Boundary.
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(contentType), defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, requestBody);
            var section = await reader.ReadNextSectionAsync(cancellationToken);
            ImageDto? imageDto = null;
            var groupDto = new GroupDto();

            // Loop through each 'Section', starting with the current 'Section'.
            while (section is not null)
            {

                // Check if the current 'Section' has a ContentDispositionHeader.
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        if (contentDisposition is not null)
                        {
                            var sectionFileName = contentDisposition.FileName.Value;
                            // use an encoded filename in case there is anything weird
                            var encodedFileName = WebUtility.HtmlEncode(Path.GetFileName(sectionFileName));

                            // read the section filename to get the content type
                            var fileExtension = Path.GetExtension(sectionFileName);

                            // now make it unique
                            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                            if (!_acceptedFileTypes.Contains(fileExtension.ToLower()))
                            {
                                _logger.LogError("file extension:{0} is not an accepted image file", fileExtension);
                                throw new ValidationException("Image", "The image is not in an accepted format");
                            }

                            var compressedImage = _imageService.TransformImageForGroupHeader(section.Body);

                            try
                            {
                                await _blobStorageProvider.UploadFileAsync(compressedImage.Image, uniqueFileName,
                                    MimeTypesMap.GetMimeType(encodedFileName), cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred uploading file to blob storage");
                                throw;
                            }

                            // trick to get the size without reading the stream in memory
                            var size = section.Body.Position;

                            // TODO MimeDetective does not work when stream has already been uploaded - figure out a solution
                            //if (fileContentTypeMatchesExtension is false)
                            //{
                            //    await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                            //    _logger.LogError("File extension:{0} does not match the file signature", fileExtension);
                            //    throw new FormatException("File extension} does not match the file signature");
                            //}

                            imageDto = new ImageDto
                            {
                                FileSizeBytes = size,
                                FileName = uniqueFileName,
                                Height = compressedImage.Height,
                                Width = compressedImage.Width,
                                IsDeleted = false,
                                MediaType = compressedImage.MediaType,
                                CreatedBy = userId,
                                CreatedAtUtc = now
                            };

                            var imageValidator = new ImageValidator(MaxFileSizeBytes);
                            var imageValidationResult = await imageValidator.ValidateAsync(imageDto, cancellationToken);
                            if (imageValidationResult.Errors.Count > 0)
                            {
                                await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                                _logger.LogError("File size:{0} is greater than the max allowed size:{1}", size, MaxFileSizeBytes);
                                throw new ValidationException(imageValidationResult);
                            }
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // if for some reason other form data is sent it would get processed here
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition?.Name.ToString().ToLowerInvariant());

                        var encoding = GetEncoding(section);
                        using var streamReader = new StreamReader(section.Body, encoding, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);

                        var value = await streamReader.ReadToEndAsync();
                        if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                        {
                            value = string.Empty;
                        }
                        formAccumulator.Append(key.Value, value);
                        if (formAccumulator.ValueCount > defaultFormOptions.ValueCountLimit)
                        {
                            _logger.LogError("GroupEdit: Form key count limit {0} exceeded.", defaultFormOptions.ValueCountLimit);
                            throw new FormatException($"Form key count limit { defaultFormOptions.ValueCountLimit } exceeded.");
                        }
                    }
                }
                // Begin reading the next 'Section' inside the 'Body' of the Request.
                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            if (formAccumulator.HasValues)
            {
                var formValues = formAccumulator.GetResults();

                // Get values from multipart form
                var nameFound = formValues.TryGetValue("name", out var name);
                if (nameFound is false)
                {
                    throw new ArgumentNullException($"Name was not provided");
                }
                var strapLineFound = formValues.TryGetValue("strapline", out var strapLine);
                if (strapLineFound is false)
                {
                    throw new ArgumentNullException($"Strap Line was not provided");
                }
                var themeFound = formValues.TryGetValue("themeid", out var theme);
                if (themeFound is false)
                {
                    throw new ArgumentNullException($"Theme was not provided");
                }

                formValues.TryGetValue("imageid", out var image);
                formValues.TryGetValue("groupOwnerId", out var groupOwner);
                formValues.TryGetValue("groupAdminId", out var groupAdmin);

                if (Guid.TryParse(theme, out var themeId) is false || themeId == new Guid())
                {
                    throw new ValidationException(nameof(GroupDto.ThemeId), "Enter the theme");
                }

                var imageId = Guid.TryParse(image, out var imageGuid) ? (Guid?)imageGuid : null;
                if (imageId.HasValue)
                {
                    if (imageId == new Guid())
                    {
                        throw new ArgumentOutOfRangeException($"Incorrect Id provided");
                    }
                }

                var groupOwnerId = Guid.TryParse(groupOwner, out var groupOwnerGuid) ? (Guid?)groupOwnerGuid : null;
                if (groupOwnerId.HasValue)
                {
                    if (groupOwnerId == new Guid())
                    {
                        throw new ArgumentOutOfRangeException($"Incorrect Id provided");
                    }
                }

                var groupAdminGuids = new List<Guid?>();
                foreach (var user in groupAdmin)
                {
                    var groupUserId = Guid.TryParse(user, out var groupUserGuid) ? (Guid?)groupUserGuid : null;

                    if (groupUserId.HasValue)
                    {
                        if (groupUserId == new Guid())
                        {
                            throw new ArgumentOutOfRangeException($"Incorrect Id provided");
                        }

                        groupAdminGuids.Add(groupUserId);
                    }
                }

                formValues.TryGetValue("isPublic", out var publicGroup);
                var isPublic = bool.TryParse(publicGroup, out var isPublicBool) ? isPublicBool : false;

                groupDto = new GroupDto
                {
                    Slug = Guid.NewGuid().ToString(),
                    Name = _htmlSanitizer.Sanitize(name),
                    Strapline = _htmlSanitizer.Sanitize(strapLine),
                    ThemeId = themeId,
                    ImageId = imageId,
                    GroupOwnerId = groupOwnerId,
                    GroupAdminUsers = groupAdminGuids,
                    ModifiedBy = userId,
                    ModifiedAtUtc = now,
                    CreatedAtUtc = now,
                    IsPublic = isPublic,
                };
            }

            return (groupDto, imageDto);
        }
        private static Encoding? GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}
