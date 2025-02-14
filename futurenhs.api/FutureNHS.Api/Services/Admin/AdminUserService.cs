﻿using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Security;

namespace FutureNHS.Api.Services.Admin
{
    public sealed class AdminUserService : IAdminUserService
    {
        private const string ListMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/list";
        private const string AddMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/add";
        private const string EditMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/edit";

        private readonly string _fqdn;
        private readonly ILogger<AdminUserService> _logger;
        private readonly IUserAdminDataProvider _userAdminDataProvider;
        private readonly IRolesDataProvider _rolesDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;

        // Notification template Ids
        private readonly string _registrationEmailId;

        public AdminUserService(ILogger<AdminUserService> logger,
            ISystemClock systemClock,
            IPermissionsService permissionsService, 
            IUserAdminDataProvider userAdminDataProvider,
            IRolesDataProvider rolesDataProvider,
            IUserCommand userCommand,
            IEmailService emailService,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig)
        {
            _permissionsService = permissionsService;
            _userAdminDataProvider = userAdminDataProvider;
            _rolesDataProvider = rolesDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userCommand = userCommand;
            _emailService = emailService;
            _fqdn = gatewayConfig.Value.FQDN;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
        }

        public async Task<MemberProfile> GetMemberAsync(Guid adminUserId, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, EditMembersRole, cancellationToken);
            var userCanViewSelf = adminUserId == targetUserId;

            if (!userCanPerformAction && !userCanViewSelf)
            {
                _logger.LogError($"Error: GetMemberAsync - User:{0} does not have access to view the target user:{1}", adminUserId, targetUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _userCommand.GetMemberAsync(targetUserId, cancellationToken);
        }

        public async Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid adminUserId, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, ListMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateDiscussionAsync - User:{0} does not have access to perform admin actions", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _userAdminDataProvider.GetMembersAsync(offset, limit, sort, cancellationToken);
        }

        public async Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchMembersAsync(Guid adminUserId, string term, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            if (term.Length is < SearchSettings.TermMinimum or > SearchSettings.TermMaximum)
            {
                throw new ArgumentOutOfRangeException(nameof(term));
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, ListMembersRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: Search Users - User:{0} does not have access to perform admin actions", adminUserId.ToString());
                throw new SecurityException($"Error: User does not have access");
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _userCommand.SearchUsers(term, offset, limit, sort, cancellationToken);
        }

        public async Task InviteMemberToGroupAndPlatformAsync(Guid adminUserId, Guid? groupId, string email, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, AddMembersRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: InviteMemberToGroupAndPlatformAsync - User:{0} does not have access to perform admin actions", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException($"Email was not provided");
            }

            if (email.Length > 254)
            {
                throw new ArgumentOutOfRangeException($"Email must be less than 254 characters");
            }

            MailAddress emailAddress;
            try
            {
                emailAddress = new MailAddress(email);
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException($"Email is not in a valid format");
            }

            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,

            };

            var registrationLink = CreateRegistrationLink();
            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };

            await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }

        public async Task<IEnumerable<RoleDto>> GetMemberRolesAsync(Guid adminUserId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, EditMembersRole, cancellationToken);
           
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetMemberRolesAsync - User:{0} does not have access to view the roles", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _rolesDataProvider.GetRolesAsync(cancellationToken);
        }

        public async Task<MemberRole> GetMemberRoleAsync(Guid adminUserId, Guid targetUserId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));
            if (Guid.Empty == targetUserId) throw new ArgumentOutOfRangeException(nameof(targetUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, EditMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetMemberRoleAsync - User:{0} does not have access to view the members role", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _userCommand.GetMembershipUsersInRoleAsync(targetUserId, cancellationToken);
        }

        public async Task UpdateMemberRoleAsync(Guid adminUserId, MemberRoleUpdate memberRoleUpdate, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == adminUserId) throw new ArgumentOutOfRangeException(nameof(adminUserId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(adminUserId, EditMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: UpdateMemberRoleAsync - User:{0} does not have access to edit a users role", adminUserId);
                throw new SecurityException($"Error: User does not have access");
            }

            var memberRole = await _userCommand.GetMembershipUsersInRoleAsync(memberRoleUpdate.MembershipUserId, cancellationToken);
            if (!memberRole.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: UpdateMemberRoleAsync - User:{0} role has changed prior to submission ", memberRoleUpdate.MembershipUserId);
                throw new PreconditionFailedExeption("Precondition Failed: User role has changed prior to submission");
            }

            if (memberRole.RoleId != memberRoleUpdate.CurrentRoleId)
            {
                _logger.LogError($"Validation Failed: UpdateMemberRoleAsync - User:{0} role id submitted does not match the users current role", memberRoleUpdate.MembershipUserId);
                throw new ValidationException(nameof(memberRoleUpdate.CurrentRoleId), "Role id submitted does not match the users current role");
            }

            await _userCommand.UpdateUserRoleAsync(memberRoleUpdate, rowVersion, cancellationToken);
        }

        private string CreateRegistrationLink()
        {
            var registrationLink = $"{_fqdn}/members/register";
            return registrationLink;
        }
    }
}
