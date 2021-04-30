﻿namespace MvcForum.Plugins.Badges
{
    using Core.Interfaces.Badges;
    using Core.Interfaces.Services;
    using Core.Models.Attributes;
    using Core.Models.Entities;

    [Id("8250f9f0-84d2-4dff-b651-c3df9e12bf2a")]
    [Name("PosterMarkAsSolution")]
    [DisplayName("Badge.PosterMarkAsSolution.Name")]
    [Description("Badge.PosterMarkAsSolution.Desc")]
    [Image("PosterMarkAsSolutionBadge.png")]
    [AwardsPoints(2)]
    public class PosterMarkAsSolutionBadge : IMarkAsSolutionBadge
    {
        private readonly IGroupService _GroupService;
        private readonly IPostService _postService;

        public PosterMarkAsSolutionBadge(IGroupService GroupService, IPostService postService)
        {
            _GroupService = GroupService;
            _postService = postService;
        }

        public bool Rule(MembershipUser user)
        {
            //Post is marked as the answer to a topic - give the post author a badge

            // Get all Groups as we want to check all the members solutions, even across
            // Groups that he no longer is allowed to access
            var cats = _GroupService.GetAll();
            var usersSolutions = _postService.GetSolutionsByMember(user.Id, cats);

            return usersSolutions.Count >= 1;
        }
    }
}