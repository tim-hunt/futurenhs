import { GetServerSideProps } from 'next'
import { getProviders } from "next-auth/react"
//import getAuthorizationUrl from 'next-auth/core/lib/oauth/authorization-url'

import authConfig from '@pages/api/auth/[...nextauth].page';
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'

const props: Partial<any> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    isRequired: false,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

                const providers: any = await getProviders();

                console.log(providers);
                //const authorizationUrl: any = await getAuthorizationUrl();

                return {
                    redirect: {
                        permanent: false,
                        destination: "https://cdsb2ctest.b2clogin.com/CDSB2CTEST.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_LocalDev&client_id=c7594320-2cfb-4c58-ae80-0203efdecaba&nonce=defaultNonce&redirect_uri=http%3A%2F%2Flocalhost%3A5000%2Fapi%2Fauth&scope=openid&response_type=code&prompt=login",
                    },                    
                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props })
            },
        }),
})

/**
 * Export page template
 */
export default () => {}
