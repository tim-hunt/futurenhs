﻿namespace MvcForum.Web.ViewModels.Topic
{
    using System;
    using System.Collections.Generic;
    using Core.Models.Entities;
    using Core.Models.General;
    using MvcForum.Core;
    using MvcForum.Core.Constants;
    using MvcForum.Core.Models.Enums;
    using Poll;
    using Post;

    public class TopicViewModel
    {
        public Topic Topic { get; set; }
        public PermissionSet Permissions { get; set; }
        public bool MemberIsOnline { get; set; }
        public bool CanViewTopic { get; set; }

        // Poll
        public PollViewModel Poll { get; set; }

        // Post Stuff
        public PostViewModel StarterPost { get; set; }

        public List<PostViewModel> Posts { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public string LastPostPermaLink { get; set; }
        public int? TotalComments { get; set; }
        // Permissions
        public bool DisablePosting { get; set; }

        // Subscription
        public bool IsSubscribed { get; set; }

        // Votes
        public int VotesUp { get; set; }

        public int VotesDown { get; set; }

        // Quote/Reply
        public string QuotedPost { get; set; }

        public Guid? ReplyTo { get; set; }
        public string ReplyToUsername { get; set; }
        public string ReplyToUsernameUrl { get; set; }

        public Guid? Thread { get; set; }
        // Stats
        public int Answers { get; set; }

        public int Views { get; set; }

        // Misc
        public bool ShowUnSubscribedLink { get; set; }

        /// <summary>
        /// Gets or sets the name of the logged in user.
        /// </summary>
        public string LoggedInUsersName { get; set; }

        /// <summary>
        /// Gets or sets the profile Url of the logged in user.
        /// </summary>
        public string LoggedInUsersUrl { get; set; }

        public GroupUserStatus GroupUserStatus { get; set; }

        /// <summary>
        /// Gets or sets the view model to create a post
        /// </summary>
        public CreateAjaxPostViewModel NewPostViewModel { get; set; }
        public bool IsMember { get; set; }
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Determine if the add comment view should be shown.
        /// </summary>
        /// <returns></returns>
        public bool CanAddComment()
        {
            return (!Permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked
                && !Permissions[ForumConfiguration.Instance.PermissionReadOnly].IsTicked
                && !Topic.Group.IsLocked
                && IsMember) || IsAdmin;
        }
    }
}