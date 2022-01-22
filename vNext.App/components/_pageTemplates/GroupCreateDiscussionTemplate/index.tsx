import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';

/**
 * Group create discussion template
 */
export const GroupCreateDiscussionTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    actions,
    text,
    image
}) => {

    const fields = forms?.[formTypes.CREATE_DISCUSSION]?.steps[0].fields ?? [];

    return (

        <GroupLayout 
            id="forum"
            user={user}
            actions={actions}
            text={text}
            image={image} 
            className="u-bg-theme-3">            
                <LayoutColumn className="c-page-body">
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={8}>
                            <FormWithErrorSummary
                                csrfToken={csrfToken}
                                fields={fields}
                                errors={{}}
                                text={{
                                    errorSummary: {
                                        body: 'There is a problem'
                                    },
                                    form: {
                                        submitButton: 'Create discussion'
                                    }
                                }} 
                                submitAction={console.log}
                                bodyClassName="u-mb-14 u-px-14 u-pt-12 u-pb-8 u-bg-theme-1"
                                submitButtonClassName="u-float-right">
                                    <h2>Create discussion</h2>
                            </FormWithErrorSummary>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </LayoutColumn>
        </GroupLayout>

    )

}
