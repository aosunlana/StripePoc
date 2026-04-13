// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { CardElement } from '@stripe/react-stripe-js';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import LoaderBase from '../../../bases/loaders/LoaderBase';
import FormBase from '../../../bases/forms/FormBase';
import { CardFormComponentStyles } from '../../../../models/components/foundations/cardForms/cardFormComponentStyles';
import useCardFormComponent from './useCardFormComponent';
import type { Stripe, StripeElements } from '@stripe/stripe-js';

interface CardFormComponentProps {
    accountId: string;
    onAdd: (stripe: Stripe | null, elements: StripeElements | null) => Promise<boolean>;
    onCancel: () => void;
}

const CardFormComponent: React.FC<CardFormComponentProps> = ({ accountId, onAdd, onCancel }) => {
    const styleRef = useRef<HTMLStyleElement>(null);
    const { stripe, processing, error, onSubmit } = useCardFormComponent(accountId, onAdd);

    return (
        <>
            <StyleBase componentRef={styleRef} styles={CardFormComponentStyles} />
            <DivisionBase className="card-form-section">
                <DivisionBase className="card-form-header">
                    <HeadingBase className="card-form-title" text="Register New Card" level={3} />
                    <ButtonBase className="btn btn-outline" text="Cancel" onClick={onCancel} />
                </DivisionBase>
                <FormBase className="card-form-container" onSubmit={onSubmit} aria-label="Payment Form">
                    <DivisionBase className="card-form-input-wrap">
                        <CardElement options={{ style: { base: { fontSize: '16px', color: '#0f172a' } } }} />
                    </DivisionBase>
                    {error && <ParagraphBase className="card-form-error" text={error} />}
                    <ButtonBase
                        className="btn btn-primary"
                        type="submit"
                        disabled={!stripe || processing}
                    >
                        {processing ? <LoaderBase size={18} /> : "Save Payment Method"}
                    </ButtonBase>
                </FormBase>
            </DivisionBase>
        </>
    );
};

export default CardFormComponent;
