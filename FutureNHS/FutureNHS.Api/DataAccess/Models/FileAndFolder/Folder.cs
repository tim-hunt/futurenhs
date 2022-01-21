﻿using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record Folder
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<FolderPathItem>? Path { get; init; }
        public Properties? FirstRegistered { get; init; }
        //public Properties LastUpdated  { get; init; }
    }
}