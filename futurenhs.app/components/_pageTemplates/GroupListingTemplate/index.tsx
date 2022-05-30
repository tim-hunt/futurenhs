import { useState } from 'react'
import Head from 'next/head'
import classNames from 'classnames'
import { useRouter } from 'next/router'

import { DynamicListContainer } from '@components/DynamicListContainer'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { GroupPageHeader } from '@components/GroupPageHeader'
import { GroupTeaser } from '@components/GroupTeaser'
import { PageBody } from '@components/PageBody'
import { PaginationWithStatus } from '@components/PaginationWithStatus'
import { getGroups } from '@services/getGroups'

import { Props } from './interfaces'

/**
 * Group listing template
 */
export const GroupListingTemplate: (props: Props) => JSX.Element = ({
    user,
    contentText,
    isGroupMember,
    groupsList,
    pagination,
}) => {
    const { pathname } = useRouter()

    const [dynamicGroupsList, setGroupsList] = useState(groupsList)
    const [dynamicPagination, setPagination] = useState(pagination)

    const shouldEnableLoadMore: boolean = true

    const {
        title,
        metaDescription,
        mainHeading,
        intro,
        secondaryHeading,
        navMenuTitle,
    } = contentText ?? {}

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalGroups, pagination } = await getGroups({
            user: user,
            isMember: isGroupMember,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize,
            },
        })

        setGroupsList([...dynamicGroupsList, ...additionalGroups])
        setPagination(pagination)
    }

    /**
     * Render
     */
    return (
        <>
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <GroupPageHeader
                    id="my-groups"
                    text={{
                        mainHeading: mainHeading,
                        description: intro,
                        navMenuTitle: navMenuTitle,
                    }}
                    navMenuList={[
                        {
                            url: '/groups',
                            text: 'My groups',
                            isActive: pathname === '/groups',
                        },
                        {
                            url: '/groups/discover',
                            text: 'Discover new groups',
                            isActive: pathname === '/groups/discover',
                        },
                    ]}
                    shouldRenderActionsMenu={false}
                    className="u-bg-theme-14"
                />
                <PageBody>
                    <LayoutColumn desktop={8}>
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        {intro && (
                            <p className="u-text-lead u-text-theme-7 u-mb-4">
                                {intro}
                            </p>
                        )}
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            {dynamicGroupsList?.map?.((teaserData, index) => {
                                return (
                                    <li key={index}>
                                        <GroupTeaser {...teaserData} />
                                    </li>
                                )
                            })}
                        </DynamicListContainer>
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            getPageAction={handleGetPage}
                            {...dynamicPagination}
                        />
                    </LayoutColumn>
                </PageBody>
            </LayoutColumnContainer>
        </>
    )
}
