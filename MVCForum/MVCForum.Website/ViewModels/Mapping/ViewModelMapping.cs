﻿namespace MvcForum.Web.ViewModels.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Admin;
    using Application;
    using Core;
    using Core.Constants;
    using Core.Interfaces.Services;
    using Core.Models;
    using Core.Models.Entities;
    using Core.Models.General;
    using MvcForum.Core.ExtensionMethods;
    using MvcForum.Core.Ioc;
    using MvcForum.Core.Models.Enums;
    using Poll;
    using Post;
    using Topic;

    public static class ViewModelMapping
    {
        #region Group


        public static Dictionary<GroupSummary, PermissionSet> GetPermissionsForGroups(IEnumerable<GroupSummary> groupSummaries,
            IRoleService roleService, MembershipRole usersRole, bool removeIfDenyAccess = false)
        {
            // Permissions
            // loop through the Groups and get the permissions
            var permissions = new Dictionary<GroupSummary, PermissionSet>();
            foreach (var groupSummary in groupSummaries)
            {
                var permissionSet = roleService.GetPermissions(groupSummary.Group, usersRole);

                // Should we add if deny access is ticked
                if (removeIfDenyAccess)
                {
                    // See if deny access is ticked
                    if (permissionSet[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                    {
                        continue;
                    }
                }

                permissions.Add(groupSummary, permissionSet);
            }
            return permissions;
        }

        public static Dictionary<Group, PermissionSet> GetPermissionsForGroups(IEnumerable<Group> Groups,
            IRoleService roleService, MembershipRole usersRole)
        {
            // Permissions
            // loop through the Groups and get the permissions
            var permissions = new Dictionary<Group, PermissionSet>();
            foreach (var Group in Groups)
            {
                var permissionSet = roleService.GetPermissions(Group, usersRole);
                permissions.Add(Group, permissionSet);
            }
            return permissions;
        }

        #endregion

        #region Membership

        public static SingleMemberListViewModel UserToSingleMemberListViewModel(MembershipUser user)
        {
            var viewModel = new SingleMemberListViewModel
            {
                IsApproved = user.IsApproved,
                Id = user.Id,
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserName = user.UserName
            };
            return viewModel;
        }

        public static MemberEditViewModel UserToMemberEditViewModel(MembershipUser user)
        {
            var viewModel = new MemberEditViewModel
            {
                IsApproved = user.IsApproved,
                Id = user.Id,
                IsLockedOut = user.IsLockedOut,
                IsBanned = user.IsBanned,
                Roles = user.Roles.Select(x => x.RoleName).ToArray(),
                UserName = user.UserName,
                IsTrustedUser = user.IsTrustedUser
            };
            return viewModel;
        }

        public static RoleViewModel RoleToRoleViewModel(MembershipRole role)
        {
            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
            return viewModel;
        }

        public static MembershipRole RoleViewModelToRole(RoleViewModel roleViewModel)
        {
            var viewModel = new MembershipRole
            {
                RoleName = roleViewModel.RoleName
            };
            return viewModel;
        }

        #endregion

        #region Settings

        public static Settings SettingsViewModelToSettings(EditSettingsViewModel settingsViewModel,
            Settings existingSettings)
        {
            //NOTE: The only reason some properties are commented out, are because those items were
            //      moved to their own page when the admin was refactored.

            existingSettings.Id = settingsViewModel.Id;
            existingSettings.ForumName = settingsViewModel.ForumName;
            existingSettings.ForumUrl = settingsViewModel.ForumUrl;
            existingSettings.IsClosed = settingsViewModel.IsClosed;
            existingSettings.EnableRSSFeeds = settingsViewModel.EnableRSSFeeds;
            existingSettings.DisplayEditedBy = settingsViewModel.DisplayEditedBy;
            existingSettings.EnableMarkAsSolution = settingsViewModel.EnableMarkAsSolution;
            existingSettings.MarkAsSolutionReminderTimeFrame = settingsViewModel.MarkAsSolutionReminderTimeFrame;
            //existingSettings.EnableSpamReporting = settingsViewModel.EnableSpamReporting;
            //existingSettings.EnableMemberReporting = settingsViewModel.EnableMemberReporting;
            existingSettings.EnableEmailSubscriptions = settingsViewModel.EnableEmailSubscriptions;
            existingSettings.ManuallyAuthoriseNewMembers = settingsViewModel.ManuallyAuthoriseNewMembers;
            existingSettings.EmailAdminOnNewMemberSignUp = settingsViewModel.EmailAdminOnNewMemberSignUp;
            existingSettings.DiscussionsPerPage = settingsViewModel.DiscussionsPerPage;
            existingSettings.PostsPerPage = settingsViewModel.PostsPerPage;
            existingSettings.ActivitiesPerPage = settingsViewModel.ActivitiesPerPage;
            existingSettings.EnablePrivateMessages = settingsViewModel.EnablePrivateMessages;
            existingSettings.MaxPrivateMessagesPerMember = settingsViewModel.MaxPrivateMessagesPerMember;
            existingSettings.PrivateMessageFloodControl = settingsViewModel.PrivateMessageFloodControl;
            existingSettings.EnableSignatures = settingsViewModel.EnableSignatures;
            existingSettings.EnablePoints = settingsViewModel.EnablePoints;
            existingSettings.PointsAllowedToVoteAmount = settingsViewModel.PointsAllowedToVoteAmount;
            existingSettings.PointsAllowedForExtendedProfile = settingsViewModel.PointsAllowedForExtendedProfile;
            existingSettings.PointsAddedPerPost = settingsViewModel.PointsAddedPerPost;
            existingSettings.PointsAddedPostiveVote = settingsViewModel.PointsAddedPostiveVote;
            existingSettings.PointsDeductedNagativeVote = settingsViewModel.PointsDeductedNagativeVote;
            existingSettings.PointsAddedForSolution = settingsViewModel.PointsAddedForSolution;
            existingSettings.AdminEmailAddress = settingsViewModel.AdminEmailAddress;
            existingSettings.NotificationReplyEmail = settingsViewModel.NotificationReplyEmail;
            existingSettings.SMTP = settingsViewModel.SMTP;
            existingSettings.SMTPUsername = settingsViewModel.SMTPUsername;
            existingSettings.SMTPPassword = settingsViewModel.SMTPPassword;
            existingSettings.Theme = settingsViewModel.Theme;
            //existingSettings.AkismentKey = settingsViewModel.AkismentKey;
            //existingSettings.EnableAkisment = settingsViewModel.EnableAkisment;
            existingSettings.SMTPPort = settingsViewModel.SMTPPort.ToString();
            //existingSettings.SpamQuestion = settingsViewModel.SpamQuestion;
            //existingSettings.SpamAnswer = settingsViewModel.SpamAnswer;
            existingSettings.SMTPEnableSSL = settingsViewModel.SMTPEnableSSL;
            //existingSettings.EnableSocialLogins = settingsViewModel.EnableSocialLogins;
            existingSettings.EnablePolls = settingsViewModel.EnablePolls;
            existingSettings.SuspendRegistration = settingsViewModel.SuspendRegistration;
            existingSettings.NewMemberEmailConfirmation = settingsViewModel.NewMemberEmailConfirmation;
            existingSettings.PageTitle = settingsViewModel.PageTitle;
            existingSettings.MetaDesc = settingsViewModel.MetaDesc;
            existingSettings.EnableEmoticons = settingsViewModel.EnableEmoticons;
            existingSettings.DisableDislikeButton = settingsViewModel.DisableDislikeButton;
            existingSettings.AgreeToTermsAndConditions = settingsViewModel.AgreeToTermsAndConditions;
            existingSettings.DisableStandardRegistration = settingsViewModel.DisableStandardRegistration;
            existingSettings.TermsAndConditions = settingsViewModel.TermsAndConditions;
            return existingSettings;
        }

        public static EditSettingsViewModel SettingsToSettingsViewModel(Settings currentSettings)
        {
            var settingViewModel = new EditSettingsViewModel
            {
                Id = currentSettings.Id,
                ForumName = currentSettings.ForumName,
                ForumUrl = currentSettings.ForumUrl,
                IsClosed = currentSettings.IsClosed,
                EnableRSSFeeds = currentSettings.EnableRSSFeeds,
                DisplayEditedBy = currentSettings.DisplayEditedBy,
                EnableMarkAsSolution = currentSettings.EnableMarkAsSolution,
                MarkAsSolutionReminderTimeFrame = currentSettings.MarkAsSolutionReminderTimeFrame ?? 0,
                //EnableSpamReporting = currentSettings.EnableSpamReporting,
                //EnableMemberReporting = currentSettings.EnableMemberReporting,
                EnableEmailSubscriptions = currentSettings.EnableEmailSubscriptions,
                ManuallyAuthoriseNewMembers = currentSettings.ManuallyAuthoriseNewMembers,
                EmailAdminOnNewMemberSignUp = currentSettings.EmailAdminOnNewMemberSignUp,
                DiscussionsPerPage = currentSettings.DiscussionsPerPage,
                PostsPerPage = currentSettings.PostsPerPage,
                ActivitiesPerPage = currentSettings.ActivitiesPerPage,
                EnablePrivateMessages = currentSettings.EnablePrivateMessages,
                MaxPrivateMessagesPerMember = currentSettings.MaxPrivateMessagesPerMember,
                PrivateMessageFloodControl = currentSettings.PrivateMessageFloodControl,
                EnableSignatures = currentSettings.EnableSignatures,
                EnablePoints = currentSettings.EnablePoints,
                PointsAllowedToVoteAmount = currentSettings.PointsAllowedToVoteAmount,
                PointsAllowedForExtendedProfile = currentSettings.PointsAllowedForExtendedProfile ?? 0,
                PointsAddedPerPost = currentSettings.PointsAddedPerPost,
                PointsAddedPostiveVote = currentSettings.PointsAddedPostiveVote,
                PointsDeductedNagativeVote = currentSettings.PointsDeductedNagativeVote,
                PointsAddedForSolution = currentSettings.PointsAddedForSolution,
                AdminEmailAddress = currentSettings.AdminEmailAddress,
                NotificationReplyEmail = currentSettings.NotificationReplyEmail,
                SMTP = currentSettings.SMTP,
                SMTPUsername = currentSettings.SMTPUsername,
                SMTPPassword = currentSettings.SMTPPassword,
                //AkismentKey = currentSettings.AkismentKey,
                //EnableAkisment = currentSettings.EnableAkisment != null && (bool)currentSettings.EnableAkisment,
                NewMemberEmailConfirmation = currentSettings.NewMemberEmailConfirmation != null &&
                                             (bool) currentSettings.NewMemberEmailConfirmation,
                Theme = currentSettings.Theme,
                SMTPPort = string.IsNullOrWhiteSpace(currentSettings.SMTPPort)
                    ? null
                    : (int?) Convert.ToInt32(currentSettings.SMTPPort),
                //SpamQuestion = currentSettings.SpamQuestion,
                //SpamAnswer = currentSettings.SpamAnswer,
                Themes = AppHelpers.GetThemeFolders(),
                SMTPEnableSSL = currentSettings.SMTPEnableSSL ?? false,
                //EnableSocialLogins = currentSettings.EnableSocialLogins ?? false,
                EnablePolls = currentSettings.EnablePolls ?? false,
                SuspendRegistration = currentSettings.SuspendRegistration ?? false,
                PageTitle = currentSettings.PageTitle,
                MetaDesc = currentSettings.MetaDesc,
                EnableEmoticons = currentSettings.EnableEmoticons == true,
                DisableDislikeButton = currentSettings.DisableDislikeButton,
                TermsAndConditions = currentSettings.TermsAndConditions,
                AgreeToTermsAndConditions = currentSettings.AgreeToTermsAndConditions ?? false,
                DisableStandardRegistration = currentSettings.DisableStandardRegistration ?? false
            };

            return settingViewModel;
        }

        #endregion

        #region Topics

        public static Dictionary<Group, PermissionSet> GetPermissionsForTopics(IEnumerable<Topic> topics,
            IRoleService roleService, MembershipRole usersRole)
        {
            // Get all the Groups for this topic collection
            var Groups = topics.Select(x => x.Group).Distinct();

            return GetPermissionsForGroups(Groups, roleService, usersRole);
        }

        public static List<TopicViewModel> CreateTopicViewModels(List<Topic> topics,
            IRoleService roleService,
            MembershipRole usersRole,
            MembershipUser loggedOnUser,
            List<Group> allowedGroups,
            Settings settings,
            IPostService postService,
            INotificationService topicNotificationService,
            IPollService pollService,
            IVoteService voteService,
            IFavouriteService favouriteService)
        {
            // Get all topic Ids
            var topicIds = topics.Select(x => x.Id).ToList();

            // Gets posts for topics
            var posts = postService.GetPostsByTopics(topicIds, allowedGroups);
            var groupedPosts = posts.ToLookup(x => x.Topic.Id);

            // Get all permissions
            var permissions = GetPermissionsForTopics(topics, roleService, usersRole);

            // Get all votes
            var votesGrouped = voteService.GetVotesByTopicsGroupedIntoPosts(topicIds);

            // Favourites grouped
            var favouritesGrouped = favouriteService.GetByTopicsGroupedIntoPosts(topicIds);

            // Create the view models
            var viewModels = new List<TopicViewModel>();
            foreach (var topic in topics)
            {
                var id = topic.Id;
                var permission = permissions[topic.Group];
                var topicPosts = groupedPosts.Contains(id) ? groupedPosts[id].ToList() : new List<Post>();

                var votes = new Dictionary<Guid, List<Vote>>();
                if (votesGrouped.ContainsKey(id))
                {
                    votes = votesGrouped[id];
                }

                var favourites = new Dictionary<Guid, List<Favourite>>();
                if (favouritesGrouped.ContainsKey(id))
                {
                    favourites = favouritesGrouped[id];
                }

                var postIds = topicPosts.Select(x => x.Id).ToList();

                viewModels.Add(CreateTopicViewModel(topic, permission, topicPosts, postIds, null, null, null, null, loggedOnUser,
                    settings, topicNotificationService, pollService, votes, favourites));
            }
            return viewModels;
        }

        public static TopicViewModel CreateTopicViewModel(Topic topic,
            PermissionSet permission,
            List<Post> posts,
            List<Guid> postIds,
            Post starterPost,
            int? pageIndex,
            int? totalCount,
            int? totalPages,
            MembershipUser loggedOnUser,
            Settings settings, 
            INotificationService topicNotificationService,
            IPollService pollService,
            Dictionary<Guid, List<Vote>> votes,
            Dictionary<Guid, List<Favourite>> favourites,
            bool getExtendedData = false)
        {
            var userIsAuthenticated = loggedOnUser != null;

            // Check for online status
            var date = DateTime.UtcNow.AddMinutes(-Constants.TimeSpanInMinutesToShowMembers);

            var viewModel = new TopicViewModel
            {
                Permissions = permission,
                Topic = topic,
                Views = topic.Views,
                DisablePosting = loggedOnUser != null,
                PageIndex = pageIndex,
                TotalCount = totalCount,
                TotalPages = totalPages,
                LastPostPermaLink = string.Concat(topic.NiceUrl, "?", Constants.PostOrderBy, "=",
                    Constants.AllPosts, "#comment-", topic.LastPost.Id),
                MemberIsOnline = topic.User.LastActivityDateUTC > date
            };

            if (starterPost == null)
            {
                starterPost = posts.FirstOrDefault(x => x.IsTopicStarter);
            }

            // Get votes for all posts
            postIds.Add(starterPost.Id);

            // Map the votes
            var startPostVotes = new List<Vote>();
            if (votes.ContainsKey(starterPost.Id))
            {
                startPostVotes= votes[starterPost.Id];
            }

            // Map the favourites
            var startPostFavs = new List<Favourite>();
            if (favourites.ContainsKey(starterPost.Id))
            {
                startPostFavs = favourites[starterPost.Id];
            }

            // Create the starter post viewmodel
            viewModel.StarterPost = CreatePostViewModel(starterPost, startPostVotes, permission, topic, loggedOnUser,
                settings, startPostFavs);

            // Map data from the starter post viewmodel
            viewModel.VotesUp = startPostVotes.Count(x => x.Amount > 0);
            viewModel.VotesDown = startPostVotes.Count(x => x.Amount < 0);
            viewModel.Answers = totalCount ?? posts.Count - 1;

            // Create the ALL POSTS view models
            viewModel.Posts =
                CreatePostViewModels(posts, votes, permission, topic, loggedOnUser, settings, favourites);

            // ########### Full topic need everything   

            if (getExtendedData)
            {
                // See if the user has subscribed to this topic or not
                var isSubscribed = userIsAuthenticated &&
                                   topicNotificationService.GetTopicNotificationsByUserAndTopic(loggedOnUser, topic).Any();
                viewModel.IsSubscribed = isSubscribed;

                // See if the topic has a poll, and if so see if this user viewing has already voted
                if (topic.Poll != null)
                {
                    // There is a poll and a user
                    // see if the user has voted or not

                    viewModel.Poll = new PollViewModel
                    {
                        Poll = topic.Poll,
                        UserAllowedToVote = permission[ForumConfiguration.Instance.PermissionVoteInPolls].IsTicked
                    };

                    var answers = pollService.GetAllPollAnswersByPoll(topic.Poll);
                    if (answers.Any())
                    {
                        var pollvotes = answers.SelectMany(x => x.PollVotes).ToList();
                        if (userIsAuthenticated)
                        {
                            viewModel.Poll.UserHasAlreadyVoted = pollvotes.Count(x => x.User.Id == loggedOnUser.Id) > 0;
                        }
                        viewModel.Poll.TotalVotesInPoll = pollvotes.Count();
                    }
                }
            }

            return viewModel;
        }

        #endregion

        #region Post

        public static PostViewModel CreatePostViewModel(Post post, List<Vote> votes, PermissionSet permission,
            Topic topic, MembershipUser loggedOnUser, Settings settings, List<Favourite> favourites)
        {
            IPostService postService = (IPostService) new UnityDependencyResolver(UnityHelper.Container).GetService(typeof(IPostService));
            var groupUser = post.Topic.Group.GroupUsers.FirstOrDefault(x => x.User.Id == loggedOnUser.Id);
            var groupUserStatus = groupUser.GetUserStatusForGroup();
            var allowedToVote = loggedOnUser != null && loggedOnUser.Id != post.User.Id;
            if (allowedToVote && settings.EnablePoints)
            {
                // We need to check if points are enabled that they have enough points to vote
                allowedToVote = loggedOnUser.TotalPoints >= settings.PointsAllowedToVoteAmount;
            }

            // Remove votes where no VotedBy has been recorded
            votes.RemoveAll(x => x.VotedByMembershipUser == null);

            var hasVotedUp = false;
            var hasVotedDown = false;
            var hasFavourited = false;
            if (loggedOnUser != null && loggedOnUser.Id != post.User.Id)
            {
                hasFavourited = favourites.Any(x => x.Member.Id == loggedOnUser.Id);
                hasVotedUp = votes.Count(x => x.Amount > 0 && x.VotedByMembershipUser.Id == loggedOnUser.Id) > 0;
                hasVotedDown = votes.Count(x => x.Amount < 0 && x.VotedByMembershipUser.Id == loggedOnUser.Id) > 0;
            }

            // Check for online status
            var date = DateTime.UtcNow.AddMinutes(-Constants.TimeSpanInMinutesToShowMembers);

            return new PostViewModel
            {
                Permissions = permission,
                Votes = votes,
                Post = post,
                ParentTopic = topic,
                AllowedToVote = allowedToVote,
                AllowedToReply = HttpContext.Current.User.Identity.IsAuthenticated == true 
                    && post.User.Id != loggedOnUser.Id 
                    && !post.IsTopicStarter 
                    && groupUserStatus == GroupUserStatus.Joined,
                MemberHasFavourited = hasFavourited,
                Favourites = favourites,
                PermaLink = string.Concat(topic.NiceUrl, "?", Constants.PostOrderBy, "=", Constants.AllPosts,
                    "#comment-", post.Id),
                MemberIsOnline = post.User.LastActivityDateUTC > date,
                HasVotedDown = hasVotedDown,
                HasVotedUp = hasVotedUp,
                VoteCount = votes.Count,
                IsTrustedUser = post.User.IsTrustedUser,
                ReplyingTo = post.InReplyTo != null ? postService.Get((Guid)post.InReplyTo) : null
            };
        }

        /// <summary>
        ///     Maps the posts for a specific topic
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="votes"></param>
        /// <param name="permission"></param>
        /// <param name="topic"></param>
        /// <param name="loggedOnUser"></param>
        /// <param name="settings"></param>
        /// <param name="favourites"></param>
        /// <returns></returns>
        public static List<PostViewModel> CreatePostViewModels(IEnumerable<Post> posts, Dictionary<Guid, List<Vote>> votes,
            PermissionSet permission, Topic topic, MembershipUser loggedOnUser, Settings settings,
            Dictionary<Guid, List<Favourite>> favourites)
        {
            var viewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                var id = post.Id;
                var postVotes = votes.ContainsKey(id) ? votes[id] : new List<Vote>();
                var postFavs = favourites.ContainsKey(id) ? favourites[id] : new List<Favourite>();
                viewModels.Add(
                    CreatePostViewModel(post, postVotes, permission, topic, loggedOnUser, settings, postFavs));
            }
            return viewModels;
        }

        /// <summary>
        ///     Maps posts from any topic must be pre filtered by checked the user has access to them
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="votes"></param>
        /// <param name="permissions"></param>
        /// <param name="loggedOnUser"></param>
        /// <param name="settings"></param>
        /// <param name="favourites"></param>
        /// <returns></returns>
        public static List<PostViewModel> CreatePostViewModels(IEnumerable<Post> posts, Dictionary<Guid, List<Vote>> votes,
            Dictionary<Group, PermissionSet> permissions, MembershipUser loggedOnUser, Settings settings,
            Dictionary<Guid, List<Favourite>> favourites)
        {
            var viewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                var id = post.Id;
                var topic = post.Topic;
                var permission = permissions[topic.Group];
                var postVotes = votes.ContainsKey(id) ? votes[id] : new List<Vote>();
                var postFavs = favourites.ContainsKey(id) ? favourites[id] : new List<Favourite>();
                viewModels.Add(
                    CreatePostViewModel(post, postVotes, permission, topic, loggedOnUser, settings, postFavs));
            }
            return viewModels;
        }

        #endregion
    }
}