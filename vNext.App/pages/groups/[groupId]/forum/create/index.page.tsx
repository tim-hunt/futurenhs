import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectCsrfToken, selectBody, selectProps, selectParam, selectUser } from '@selectors/context';
import { postGroupDiscussion } from '@services/postGroupDiscussion';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createDiscussionForm } from '@formConfigs/create-discussion';
import { GroupCreateDiscussionTemplate } from '@components/_pageTemplates/GroupCreateDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces';

const routeId: string = 'fcf3d540-9a55-418c-b317-a14146ae075f';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const csrfToken: string = selectCsrfToken(context);
                const formPost: any = selectBody(context);

                let props: Props = selectProps(context);

                props.csrfToken = csrfToken;
                props.forms = {
                    [createDiscussionForm.id]: {
                        config: createDiscussionForm
                    }
                };

                if(!props.actions?.includes(actionConstants.GROUPS_DISCUSSIONS_ADD)){

                    return {
                        redirect: {
                            permanent: false,
                            destination: '/'
                        }
                    }

                }

                if(formPost){

                    try {

                        const submission = await postGroupDiscussion({
                            groupId: groupId,
                            user: user,
                            csrfToken: csrfToken,
                            body: {
                                formId: createDiscussionForm.id,
                                ...formPost
                            }
                        });

                    } catch(error){

                        if(error.name === 'ServiceError' && error.data?.status === 400){

                            props.forms[createDiscussionForm.id].validationErrors = [error.data.body];

                        }

                    }

                }

                /**
                 * Return data to page template
                 */
                return {
                    props: getJsonSafeObject({
                        object: props
                    })
                }

            }
        })
});

/**
 * Export page template
 */
export default GroupCreateDiscussionTemplate;