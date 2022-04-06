import { useState } from 'react';

import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { selectFormDefaultFields, selectFormErrors, selectFormInitialValues } from '@selectors/forms';
import { formTypes } from '@constants/forms';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { postSiteUserInvite } from '@services/postSiteUserInvite';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Admin invite user template
 */
export const AdminUsersInviteTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    routes,
    user,
    contentText,
    services = {
        postSiteUserInvite: postSiteUserInvite
    }
}) => {

    const [errors, setErrors] = useState(selectFormErrors(forms, formTypes.INVITE_USER));

    const fields = selectFormDefaultFields(forms, formTypes.INVITE_USER);
    const initialValues = selectFormInitialValues(forms, formTypes.INVITE_USER);

    const { secondaryHeading } = contentText ?? {};

    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        try {

            await services.postSiteUserInvite({ user, body: formData as any });

            return Promise.resolve({});

        } catch (error) {

            const errors: FormErrors = getServiceErrorDataValidationErrors(error) || getGenericFormError(error);

            setErrors(errors);

            return Promise.resolve(errors);

        }

    };

    /**
     * Render
     */
    return (

        <LayoutColumnContainer>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formId={formTypes.CREATE_DISCUSSION}
                            fields={fields}
                            initialValues={initialValues}
                            errors={errors}
                            text={{
                                errorSummary: {
                                    body: 'There is a problem'
                                },
                                form: {
                                    submitButton: 'Send invite',
                                    cancelButton: 'Discard invite'
                                }
                            }}
                            submitAction={handleSubmit}
                            cancelHref={routes.siteRoot}
                            shouldClearOnSubmitSuccess={true}
                            bodyClassName="u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1">
                                <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
        </LayoutColumnContainer>

    )

}
