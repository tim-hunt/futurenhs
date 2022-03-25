import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { selectTheme } from '@selectors/themes';
import { RichText } from '@components/RichText';
import { Link } from '@components/Link';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { Theme } from '@appTypes/theme';

import { Props } from './interfaces';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    routes,
    actions,
    themeId
}) => {

    const router = useRouter();

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);
    const isInEditMode: boolean = Boolean(router.query.edit);

    const { background, accent }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
        adminCallOut: classNames('nhsuk-inset-text u-m-0 u-max-w-full', `u-border-l-theme-${background}`),
        adminCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        adminCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 tablet:u-mt-0'),
        previewButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 tablet:u-mt-0 tablet:u-mr-5'),
        publishButton: classNames('c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0')
    };

    return (

        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            {(isGroupAdmin && !isInEditMode) &&
                <LayoutColumnContainer>
                    <LayoutColumn tablet={9}>
                        <div className={generatedClasses.adminCallOut}>
                            <p className={generatedClasses.adminCallOutText}>You are a Group Admin of this page. Please click edit to switch to editing mode.</p>
                        </div>
                    </LayoutColumn>
                    <LayoutColumn tablet={3} className="u-flex u-items-center">
                        <Link href={`${routes.groupRoot}?edit=true`}><a className={generatedClasses.adminCallOutButton}>Edit page</a></Link>
                    </LayoutColumn>
                </LayoutColumnContainer>
            }
            {(isGroupAdmin && isInEditMode) &&
                <div className={generatedClasses.adminCallOut}>
                    <LayoutColumnContainer className="u-mb-6">
                        <LayoutColumn tablet={6}><h2 className="nhsuk-heading-l u-m-0">Editing group homepage</h2></LayoutColumn>
                        <LayoutColumn tablet={6} className="u-flex u-items-center">
                            <button className={generatedClasses.previewButton}>Preview page</button>
                            <button className={generatedClasses.publishButton}>Publish group page</button>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                    <RichText 
                        wrapperElementType="div" 
                        bodyHtml="Welcome to your group homepage. You are currently in editing mode. You can save a draft at any time, preview your page, or publish your changes. Once published, you can edit your page in the group actions. For more information and help, see our quick guide.
For some inspiration, visit our knowledge hub." 
                        className="u-text-lead u-text-theme-7" />
                </div>
            }
        </LayoutColumn>

    )

}