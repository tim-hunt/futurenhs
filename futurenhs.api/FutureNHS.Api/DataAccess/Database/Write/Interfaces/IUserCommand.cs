﻿using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IUserCommand
    {
        Task<UserDto> GetUserAsync(Guid userId, CancellationToken cancellationToken);
        Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken);
        Task UpdateUserAsync(MemberDto userDto, byte[] rowVersion, CancellationToken cancellationToken);
        Task<MemberProfile> GetMemberAsync(Guid id, CancellationToken CancellationToken);
        Task<MemberRole> GetMembershipUsersInRoleAsync(Guid userId, CancellationToken cancellationToken);
        Task UpdateUserRoleAsync(MemberRoleUpdate memberRoleUpdate, byte[] rowVersion, CancellationToken cancellationToken);
        Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(string term, uint offset, uint limit,
            string sort, CancellationToken cancellationToken);
    }
}
