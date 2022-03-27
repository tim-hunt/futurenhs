import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { selectTheme } from '@selectors/themes';
import { PageManager } from '@components/PageManager';
import { RichText } from '@components/RichText';
import { NoScript } from '@components/NoScript';
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

    const [isInEditMode, setIsInEditMode] = useState(Boolean(router.query.edit));
    const [isClient, setIsClient] = useState(false);

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);
    const { background, accent }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
        adminCallOut: classNames('nhsuk-inset-text u-m-0 u-max-w-full', `u-border-l-theme-${background}`),
        adminCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        adminCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 tablet:u-mt-0'),
        previewButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 tablet:u-mt-0 tablet:u-mr-5'),
        publishButton: classNames('c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0')
    };

    const handleSetToEditMode = (): void => {
        setIsInEditMode(true);
    }

    const handlePublishUpdate = (): void => {
        setIsInEditMode(false);
    }

    useDynamicElementClassName({
        elementSelector: isInEditMode ? 'main' : null,
        addClass: 'u-bg-theme-1',
        removeClass: 'u-bg-theme-3'
    });

    useEffect(() => {

        setIsClient(true);

    }, []);

    return (

        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            {isGroupAdmin &&
                <NoScript headingLevel={2} text={{
                    heading: 'Important',
                    body: 'JavaScript must be enabled in your browser to manage this page'
                }} />
            }
            {(isClient && isGroupAdmin && !isInEditMode) &&
                <LayoutColumnContainer>
                    <LayoutColumn tablet={9}>
                        <div className={generatedClasses.adminCallOut}>
                            <p className={generatedClasses.adminCallOutText}>You are a Group Admin of this page. Please click edit to switch to editing mode.</p>
                        </div>
                    </LayoutColumn>
                    <LayoutColumn tablet={3} className="u-flex u-items-center">
                        <button className={generatedClasses.adminCallOutButton} onClick={handleSetToEditMode}>Edit page</button>
                    </LayoutColumn>
                </LayoutColumnContainer>
            }
            {(isClient && isGroupAdmin && isInEditMode) &&
                <>
                    <div className={generatedClasses.adminCallOut}>
                        <LayoutColumnContainer className="u-mb-6">
                            <LayoutColumn tablet={6}><h2 className="nhsuk-heading-l u-m-0">Editing group homepage</h2></LayoutColumn>
                            <LayoutColumn tablet={6} className="tablet:u-flex u-items-center">
                                <button className={generatedClasses.previewButton}>Preview page</button>
                                <button className={generatedClasses.publishButton} onClick={handlePublishUpdate}>Publish group page</button>
                            </LayoutColumn>
                        </LayoutColumnContainer>
                        <RichText
                            wrapperElementType="div"
                            bodyHtml="Welcome to your group homepage. You are currently in editing mode. You can save a draft at any time, preview your page, or publish your changes. Once published, you can edit your page in the group actions. For more information and help, see our quick guide.
            For some inspiration, visit our knowledge hub."
                            className="u-text-lead u-text-theme-7" />
                    </div>
                    <div className="u-mt-14">
                        <PageManager />
                    </div>
                </>
            }
        </LayoutColumn>

    )

}