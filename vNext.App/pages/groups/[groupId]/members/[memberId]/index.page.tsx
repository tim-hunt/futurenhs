import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupMember } from '@services/getGroupMember';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectUser, selectGroupId, selectMemberId, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberTemplate } from '@components/_pageTemplates/GroupMemberTemplate';

const routeId: string = '4502d395-7c37-4e80-92b7-65886de858ef';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                let { props } = context;

                const groupId: string = selectGroupId(context);
                const memberId: string = selectMemberId(context);
                const user: User = selectUser(context);
                const locale: string = selectLocale(context);

                /**
                 * Get data from services
                 */
                try {

                    const [
                        memberData,
                        pageTextContent
                    ] = await Promise.all([
                        getGroupMember({
                            groupId: groupId,
                            user: user,
                            memberId: memberId
                        }),
                        getPageTextContent({
                            id: routeId,
                            locale: locale
                        })
                    ]);

                    if(getServiceResponsesWithStatusCode({
                        serviceResponseList: [memberData],
                        statusCode: 404
                    }).length > 0){

                        return {
                            notFound: true
                        }

                    }

                    props.member = memberData.data;
                    props.text = Object.assign({}, props.text, pageTextContent.data ?? {});
                
                } catch (error) {
                    
                    //props.errors = error;

                }

                /**
                 * Return data to page template
                 */
                return {
                    props: props
                }

            }
        })
});

/**
 * Export page template
 */
 export default GroupMemberTemplate;