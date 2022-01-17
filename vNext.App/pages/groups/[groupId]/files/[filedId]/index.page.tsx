import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
//import { getGroupFiles } from '@services/getGroupFolders';
import { selectUser, selectFileId, selectGroupId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFileTemplate } from '@components/_pageTemplates/GroupFileTemplate';
import { Props } from '@components/_pageTemplates/GroupFileTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = 'b74b9b6b-0462-4c2a-8859-51d0df17f68f';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const fileId: string = selectFileId(context);
    const user: User = selectUser(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        fileId: null,
        file: null,
        content: null,
        image: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData
        ] = await Promise.all([
            getGroup({
                groupId: groupId
            })
        ]);

        if(getServiceResponsesWithStatusCode({
            serviceResponseList: [groupData],
            statusCode: 404
        }).length > 0){

            return {
                notFound: true
            }

        }

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;
    
    } catch (error) {

        console.log(error);
        
    }

    /**
     * Return data to page template
     */
    return {
        props: props
    }

});

/**
 * Export page template
 */
export default GroupFileTemplate;