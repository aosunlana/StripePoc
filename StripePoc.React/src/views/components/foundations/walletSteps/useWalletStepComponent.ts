// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState, useEffect } from 'react';
import { walletViewService } from '../../../../../features/wallet/wallet.module';
import { PaymentMethodView } from '../../../../models/views/PaymentMethodView';
import type { Stripe, StripeElements } from '@stripe/stripe-js';

const useWalletStepComponent = (accountId: string) => {
    const [paymentMethods, setPaymentMethods] = useState<PaymentMethodView[]>([]);
    const [showAddCard, setShowAddCard] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!accountId) return;
        setLoading(true);
        walletViewService.retrievePaymentMethodsViewAsync(accountId)
            .then(setPaymentMethods)
            .catch(() => setError('Failed to load payment methods.'))
            .finally(() => setLoading(false));
    }, [accountId]);

    const onShowAddCard = () => setShowAddCard(true);
    const onHideAddCard = () => setShowAddCard(false);

    const onAddCard = async (stripe: Stripe | null, elements: StripeElements | null): Promise<boolean> => {
        if (!stripe || !elements) return false;
        
        setLoading(true);
        setError(null);
        try {
            const newMethod = await walletViewService.addCardViewAsync(accountId, stripe, elements);
            setPaymentMethods(prev => [...prev, newMethod]);
            setShowAddCard(false);
            return true;
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'Failed to add card.';
            setError(message);
            return false;
        } finally {
            setLoading(false);
        }
    };

    return {
        paymentMethods,
        showAddCard,
        loading,
        error,
        onShowAddCard,
        onHideAddCard,
        onAddCard,
    };
};

export default useWalletStepComponent;
