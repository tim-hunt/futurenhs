import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getRouteToParam2 } from '@helpers/routing/getRouteToParam'
import { Hof } from '@appTypes/hof'

export const withRoutes: Hof = (context) => {

    /**
     * Set up current routing data relative to context
     */
    try {
        const groupIndexRoute: string = getRouteToParam2({
            route: context.resolvedUrl,
            param: context.params?.groupId,
        })

            props.routes = getJsonSafeObject({
                object: {
                    siteRoot: '/',
                    usersRoot: '/users',
                    adminRoot: '/admin',
                    adminUsersRoot: '/admin/users',
                    adminUsersInvite: '/admin/users/invite',
                    adminGroupsRoot: '/admin/groups',
                    adminGroupsCreate: '/admin/users/create',
                    authApiSignInAzureB2C: '/api/auth/signin/azure-ad-b2c',
                    authApiSignOut: '/api/auth/signout/azure-ad-b2c',
                    authSignIn: '/auth/signin',
                    authSignOut: '/auth/signout',
                    groupsRoot: '/groups',
                    groupsDiscover: '/groups/discover',
                    groupRoot: groupIndexRoute ? groupIndexRoute : null,
                    groupUpdate: groupIndexRoute
                        ? `${groupIndexRoute}/update`
                        : null,
                groupFilesRoot: groupIndexRoute
                    ? `${groupIndexRoute}/files`
                    : null,
                groupMembersRoot: groupIndexRoute
                    ? `${groupIndexRoute}/members`
                    : null,
                groupAboutRoot: groupIndexRoute
                    ? `${groupIndexRoute}/about`
                    : null,
            },
        })
    } catch (error) {
        return handleSSRErrorProps({ props: context.page.props, error })
    }

}
