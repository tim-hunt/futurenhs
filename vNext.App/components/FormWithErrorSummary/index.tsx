import { useRef, useState, useCallback } from 'react';
import classNames from 'classnames';

import { ErrorSummary } from '@components/ErrorSummary';
import { Form } from '@components/Form';

import { Props } from './interfaces';

export const FormWithErrorSummary: (props: Props) => JSX.Element = ({
    action,
    method = 'POST',
    csrfToken,
    initialValues = {},
    fields,
    errors,
    submitAction,
    cancelHref,
    text,
    children,
    className,
    bodyClassName,
    submitButtonClassName
}) => {

    const errorSummaryRef: any = useRef();
    const [validationErrors, setValidationErrors] = useState(errors ? errors : []);
    const relatedNames: Array<string> = fields?.map(({ name }) => name);

    const handleChange = useCallback(({ errors, submitErrors, submitFailed, modifiedSinceLastSubmit }): any => {

        const hasErrors: boolean = errors && Object.keys(errors).length > 0;
        const hasSubmitErrors: boolean = submitErrors && Object.keys(submitErrors).length > 0;

        setTimeout(() => {

            const errorsAsArray: Array<any> = errors ? Object.keys(errors).map((key: string) => ({ [key]: errors[key] })) : [];
            const submitErrorsAsArray: Array<any> = submitErrors ? Object.keys(submitErrors).map((key: string) => ({ [key]: submitErrors[key] })) : [];
            const errorsToUse: Array<any> = submitFailed ? hasErrors ? errorsAsArray : submitErrorsAsArray : [];
            const shouldFocusSummary: boolean = submitFailed && modifiedSinceLastSubmit && (hasErrors || hasSubmitErrors);

            setValidationErrors(errorsToUse);

            if(shouldFocusSummary){

                errorSummaryRef.current?.focus();

            }

        }, 0);

    }, [errors]);

    const { errorSummary: errorSummaryText, form: formText } = text ?? {};

    const generatedClasses: any = {
        form: classNames('c-form', className)
    };

    return (

        <>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={validationErrors}
                relatedNames={relatedNames}
                text={errorSummaryText} 
                className="u-mb-6"/>
                    {children}
            <Form 
                action={action}
                method={method}
                initialValues={initialValues}
                csrfToken={csrfToken}
                fields={fields}
                text={formText}
                className={generatedClasses.form}
                bodyClassName={bodyClassName}
                submitButtonClassName={submitButtonClassName}
                cancelHref={cancelHref}
                changeAction={handleChange}
                submitAction={submitAction} />
        </>
        
    )

}
