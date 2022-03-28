﻿using FutureNHS.Api.Configuration;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IGroupImageService, ImageService>();
            services.AddScoped<IGroupMembershipService, GroupMembershipService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserImageService, ImageService>();

            return services;
        }
    }
}
