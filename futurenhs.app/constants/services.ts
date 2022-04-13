export const enum services {
    GET_GROUPS = 'getGroups',
    GET_GROUP = 'getGroup',
    GET_GROUP_ACTIONS = 'getGroupActions',
    GET_GROUP_DISCUSSIONS = 'getGroupDiscussions',
    GET_GROUP_DISCUSSION = 'getGroupDiscussion',
    GET_GROUP_DISCUSSION_COMMENTS = 'getGroupDiscussionComments',
    GET_GROUP_DISCUSSION_REPLIES = 'getGroupDiscussionReplies',
    GET_GROUP_DISCUSSION_COMMENTS_WITH_REPLIES = 'getGroupDiscussionCommentsWithReplies',
    GET_GROUP_FOLDER_CONTENTS = 'getGroupFolderContents',
    GET_GROUP_FOLDER = 'getGroupFolder',
    GET_GROUP_FILE = 'getGroupFile',
    GET_GROUP_FILE_VIEW = 'getGroupFileView',
    GET_GROUP_MEMBERS = 'getGroupMembers',
    GET_PENDING_GROUP_MEMBERS = 'getGroupPendingMembers',
    GET_GROUP_MEMBER = 'getGroupMember',
    GET_PAGE_TEXT_CONTENT = 'getPageTextContent',
    GET_SEARCH_RESULTS = 'getSearchResults',
    GET_SITE_ACTIONS = 'getSiteActions',
    GET_SITE_USER = 'getSiteUser',
    GET_SITE_USERS = 'getSiteUsers',
    GET_SITE_USERS_BY_TERM = 'getSiteUsersByTerm',
    GET_USER = 'getUser',
    POST_SITE_USER_INVITE = 'postSiteUserInvite',
    POST_GROUP = 'postGroup',
    POST_GROUP_DISCUSSION = 'postGroupDiscussion',
    POST_GROUP_DISCUSSION_COMMENT = 'postGroupDiscussionComment',
    POST_GROUP_DISCUSSION_COMMENT_REPLY = 'postGroupDiscussionCommentReply',
    POST_GROUP_FOLDER = 'postGroupFolder',
    POST_GROUP_FILE = 'postGroupFile',
    POST_GROUP_MEMBERSHIP = 'postGroupMembership',
    PUT_GROUP = 'putGroup',
    PUT_GROUP_FOLDER = 'putGroupFolder',
    PUT_GROUP_DISCUSSION_COMMENT_LIKE = 'putGroupDiscussionCommentLike',
    PUT_SITE_USER = 'putSiteUser',
    DELETE_GROUP_MEMBERSHIP = 'deleteGroupMembership',
    DELETE_GROUP_FOLDER = 'deleteGroupFolder',
}
