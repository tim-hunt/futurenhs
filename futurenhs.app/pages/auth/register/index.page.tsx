import { GetServerSideProps } from 'next'
import { getSession } from 'next-auth/react'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import {
    selectCsrfToken,
    selectFormData,
    selectMultiPartFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'
import { getSiteUserRole } from '@services/getSiteUserRole'
import { formTypes } from '@constants/forms'
import { FormConfig, FormOptions } from '@appTypes/form'
import { setFormConfigOptions } from '@helpers/util/form'
import { withTokens } from '@hofs/withTokens'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { postSiteUser } from '@services/postSiteUser'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { User } from '@appTypes/user'
import { actions } from '@constants/actions'
import { putSiteUserRole } from '@services/putSiteUserRole'
import { getSiteUserRoles } from '@services/getSiteUserRoles'
import { selectForm } from '@selectors/forms'
import formConfigs from '@formConfigs/index'
import { AuthRegisterTemplate } from '@components/_pageTemplates/AuthRegisterTemplate'
import { Props } from '@components/_pageTemplates/AuthRegisterTemplate/interfaces'

const routeId: string = '578f216f-cee3-4e69-9e4f-90be0e2bc4e8'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withRoutes({
    props,
    getServerSideProps: withTokens({
        props,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {
                const session = await getSession(context)

                /**
                 * Hide breadcrumbs
                 */
                ;(props as any).breadCrumbList = []

                /**
                 * Redirect to site root if already signed out
                 */
                if (!session) {
                    return {
                        redirect: {
                            permanent: false,
                            destination: `${process.env.APP_URL}/auth/signin`,
                        },
                    }
                }
                const csrfToken: string = selectCsrfToken(context)
                const currentValues: any = selectFormData(context)
                const submission: any = selectMultiPartFormData(context)
                const requestMethod: requestMethods =
                    selectRequestMethod(context)

                props.layoutId = layoutIds.ADMIN
                props.forms = {
                    [formTypes.REGISTER_SITE_USER]: {},
                }

                const profileForm: FormConfig =
                    props.forms[formTypes.REGISTER_SITE_USER]

                /**
                 * Get data from services
                 */
                try {
                    /**
                     * Handle server-side profile form post
                     */
                    if (submission && requestMethod === requestMethods.POST) {
                        if (
                            currentValues.body?.['_form-id'] ===
                            formTypes.REGISTER_SITE_USER
                        ) {
                            profileForm.initialValues = currentValues.getAll()

                            const headers = {
                                ...getStandardServiceHeaders({
                                    csrfToken,
                                }),
                                ...submission.getHeaders(),
                            }

                            await postSiteUser({
                                headers,
                                body: submission,
                            })
                        }

                        return {
                            redirect: {
                                permanent: false,
                                destination: `${props.routes.usersRoot}/register`,
                            },
                        }
                    }
                } catch (error: any) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        profileForm.errors = validationErrors
                    } else {
                        return handleSSRErrorProps({ props, error })
                    }
                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props })
            },
        }),
    }),
})
/**
 * Export page template
 */
export default AuthRegisterTemplate
