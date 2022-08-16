import { Link } from '@components/Link'
import { PageBody } from '@components/PageBody'
import { RichText } from '@components/RichText'

import { Props } from './interfaces'
import { actions as actionsConstants } from '@constants/actions'
import { selectForm, selectFormErrors } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form'
import { formTypes } from '@constants/forms'
import { useRef, useState } from 'react'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Avatar } from '@components/Avatar'
import { initials } from '@helpers/formatters/initials'
import { Form } from '@components/Form'
import { ErrorSummary } from '@components/ErrorSummary'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { postSiteUser } from '@services/postSiteUser'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { useRouter } from 'next/router'
import { putSiteUserRole } from '@services/putSiteUserRole'
import { useFormConfig } from '@hooks/useForm'

/**
 * Auth unregistered template
 */
export const AuthRegisterTemplate: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    forms,
    csrfToken,
    routes,
    etag,
}) => {
    const errorSummaryRef: any = useRef()
    const { authSignOut } = routes
    const profileFormConfig: FormConfig = useFormConfig(
        formTypes.REGISTER_SITE_USER,
        forms[formTypes.REGISTER_SITE_USER]
    )

    const [errors, setErrors] = useState(
        Object.assign({}, selectFormErrors(forms, formTypes.REGISTER_SITE_USER))
    )

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side update submission for profile details
     */
    const handleProfileSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const etagToUse: string =
                typeof etag === 'object' ? etag.profileEtag : etag

            const headers = getStandardServiceHeaders({
                csrfToken,
                etag: etagToUse,
            })

            postSiteUser({
                body: formData,
                headers,
            })
                .then(() => {
                    setErrors({})
                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }
    const { mainHeading, bodyHtml, signOut } = contentText ?? {}

    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={6}>
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        className="u-mb-10"
                    />
                    <RichText
                        wrapperElementType="div"
                        className="u-mb-10"
                        bodyHtml={bodyHtml}
                    />
                    <Form
                        csrfToken={csrfToken}
                        formConfig={profileFormConfig}
                        text={{
                            submitButton: 'Next',
                            cancelButton: 'Cancel',
                        }}
                        submitAction={handleProfileSubmit}
                        cancelHref={`${authSignOut}`}
                        validationFailAction={handleValidationFailure}
                    />
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )
}
