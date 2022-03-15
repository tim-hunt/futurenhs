import { Image } from '@appTypes/image';

export interface Props {
    id?: string;
    csrfToken: string;
    initialErrors?: any;
    commentId: string;
    children?: any;
    image?: Image;
    text: any;
    userProfileLink: string;
    date: string;
    shouldEnableReplies?: boolean;
    replyValidationFailAction?: any;
    replySubmitAction: any;
    shouldEnableLikes?: boolean;
    likeAction?: any;
    likeCount?: number;
    isLiked?: boolean;
    className?: string;
}

