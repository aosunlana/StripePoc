// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState } from 'react';
import { actionViewService } from '../../../../../features/action/action.module';
import type { AccountView } from '../../../../models/views/AccountView';
import type { ServiceSelectionView } from '../../../../models/views/ServiceSelectionView';
import type { PaymentLifecycleEventView } from '../../../../models/views/PaymentLifecycleEventView';

const useActionStepComponent = (account: AccountView, selection: ServiceSelectionView) => {
    const [loading, setLoading] = useState<'direct' | 'quote-sub' | 'quote-one' | null>(null);
    const [error, setError] = useState<string | null>(null);

    const onDirectCharge = async (): Promise<PaymentLifecycleEventView | null> => {
        setLoading('direct');
        setError(null);
        try {
            return await actionViewService.directChargeViewAsync(account.id, selection);
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Direct charge failed.');
            return null;
        } finally {
            setLoading(null);
        }
    };

    const onRequestSubscriptionQuote = async (): Promise<PaymentLifecycleEventView | null> => {
        setLoading('quote-sub');
        setError(null);
        try {
            return await actionViewService.requestSubscriptionQuoteViewAsync(account.id, selection);
        } catch (err: any) {
            setError(err.message || 'Subscription quote request failed.');
            return null;
        } finally {
            setLoading(null);
        }
    };

    const onRequestOneTimeQuote = async (): Promise<PaymentLifecycleEventView | null> => {
        setLoading('quote-one');
        setError(null);
        try {
            return await actionViewService.requestOneTimeQuoteViewAsync(account.id, selection);
        } catch (err: any) {
            setError(err.message || 'One-time quote request failed.');
            return null;
        } finally {
            setLoading(null);
        }
    };

    return { loading, error, onDirectCharge, onRequestSubscriptionQuote, onRequestOneTimeQuote };
};

export default useActionStepComponent;
