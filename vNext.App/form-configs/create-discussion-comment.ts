import { formTypes } from '@constants/forms';
import { Form } from '@appTypes/form';

export const createDiscussionCommentForm: Form = {
    id: formTypes.CREATE_DISCUSSION_COMMENT,
    steps: [
        {
            fields: [
                {
                    name: 'discussion-comment',
                    text: {
                        label: 'Your comment'
                    },
                    component: 'textArea',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter your comment'
                        }
                    ]
                }
            ]
        }
    ]
};