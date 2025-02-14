﻿using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public record CommentNavProperty
    {
        public Guid? Id { get; init; }
        public string? AtUtc { get; init; }
        public UserNavProperty? By { get; init; }
    }
}
