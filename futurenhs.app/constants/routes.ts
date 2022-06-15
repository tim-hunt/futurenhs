export const enum routes {
    HOME = '/',
    ADMIN = '/admin',
    ADMIN_USERS = '/admin/users',
    ADMIN_GROUPS = '/admin/groups',
    USERS = '/users',
    SIGN_IN = '/auth/signin',
    SIGN_OUT = '/auth/signout',
    GROUPS = '/groups',
    ACCESSIBILITY = '/accessibility-statement',
    CONTACT_US = '/contact-us',
    COOKIES = '/cookies',
    PRIVACY_POLICY = '/privacy-policy',
    TERMS_AND_CONDITIONS = '/terms-and-conditions',
}

export const enum queryParams {
    RETURNURL = 'returnUrl',
    EDIT = 'edit',
}

export const enum routeParams {
    GROUPID = 'groupId',
    FOLDERID = 'folderId',
    FILEID = 'fileId',
    MEMBERID = 'memberId',
    DISCUSSIONID = 'discussionId',
    USERID = 'userId',
}

export const enum layoutIds {
    BASE = 'base',
    GROUP = 'group',
    ADMIN = 'admin',
}

export const enum groupTabIds {
    INDEX = 'index',
    FORUM = 'forum',
    FILES = 'files',
    MEMBERS = 'members',
    ABOUT = 'about',
}
