import { Page } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'

declare interface ContentText extends GroupsPageTextContent {
    signOut: string
}

export interface Props extends Page {
    contentText: ContentText
}
