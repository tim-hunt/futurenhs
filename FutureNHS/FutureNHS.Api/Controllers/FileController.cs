using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly IFileAndFolderDataProvider _fileAndFolderDataProvider;
        private readonly IFileService _fileService;
        private readonly IPermissionsService _permissionsService;

        public FileController(ILogger<FileController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IPermissionsService permissionsService, IFileService fileService)
        {
            _logger = logger;
            _fileAndFolderDataProvider = fileAndFolderDataProvider;
            _fileService = fileService;
            _permissionsService = permissionsService;
        }

        [HttpGet]
        [Route("users/{userId}/groups/{slug}/files/{id:guid}")]

        public async Task<IActionResult> GetFileAsync(Guid id, CancellationToken cancellationToken)
        {
            var file = await _fileAndFolderDataProvider.GetFileAsync(id, cancellationToken);

            if (file is null)
            { 
                return NotFound();
            }

            return Ok(file);
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/files")]

        public async Task<IActionResult> CreateFileAsync(Guid userId,string slug, Guid folderId, FutureNHS.Api.Models.File.File file, CancellationToken cancellationToken)
        {
            await _fileService.CreateFileAsync(userId,slug, folderId, file, cancellationToken);

            return Ok(file);
        }
    }
}