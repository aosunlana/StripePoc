// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState } from 'react';
import { useStripe, useElements } from '@stripe/react-stripe-js';
import type { Stripe, StripeElements } from '@stripe/stripe-js';

const useCardFormComponent = (accountId: string, onAdd: (stripe: Stripe | null, elements: StripeElements | null) => Promise<boolean>) => {
    const stripe = useStripe();
    const elements = useElements();
    const [processing, setProcessing] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setProcessing(true);
        setError(null);
        try {
            const success = await onAdd(stripe, elements);
            if (success) {
                // handle success
            }
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to register card.';
            setError(message);
        } finally {
            setProcessing(false);
        }
    };

    return { stripe, processing, error, onSubmit };
};

export default useCardFormComponent;
