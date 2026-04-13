// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState, useEffect } from 'react';
import { identityViewService } from '../../../../../features/identity/identity.module';
import type { BusinessView } from '../../../../models/views/BusinessView';
import type { AccountView } from '../../../../models/views/AccountView';

const useIdentityStepComponent = () => {
    const [businesses, setBusinesses] = useState<BusinessView[]>([]);
    const [accounts, setAccounts] = useState<AccountView[]>([]);
    const [selectedBusiness, setSelectedBusiness] = useState<BusinessView | null>(null);
    const [selectedAccount, setSelectedAccount] = useState<AccountView | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        setLoading(true);
        identityViewService.retrieveAllBusinessesViewAsync()
            .then(setBusinesses)
            .catch(() => setError('Failed to load businesses.'))
            .finally(() => setLoading(false));
    }, []);

    const onBusinessSelect = async (businessId: string) => {
        const business = businesses.find(b => b.id === businessId) ?? null;
        setSelectedBusiness(business);
        setSelectedAccount(null);
        if (!business) return;

        setLoading(true);
        try {
            const accs = await identityViewService.retrieveAccountsByBusinessIdViewAsync(business.id);
            setAccounts(accs);
        } catch {
            setError('Failed to load accounts.');
        } finally {
            setLoading(false);
        }
    };

    const onAccountSelect = (accountId: string) => {
        const account = accounts.find(a => a.id === accountId) ?? null;
        setSelectedAccount(account);
    };

    const onVerifyIdentity = async (): Promise<boolean> => {
        if (!selectedAccount) return false;
        setLoading(true);
        setError(null);
        try {
            await identityViewService.verifyIdentityViewAsync(selectedAccount.id);
            return true;
        } catch {
            setError('Identity verification failed. Please try again.');
            return false;
        } finally {
            setLoading(false);
        }
    };

    const onReset = () => {
        setSelectedBusiness(null);
        setSelectedAccount(null);
        setAccounts([]);
        setError(null);
    };

    return {
        businesses,
        accounts,
        selectedBusiness,
        selectedAccount,
        loading,
        error,
        onBusinessSelect,
        onAccountSelect,
        onVerifyIdentity,
        onReset,
    };
};

export default useIdentityStepComponent;
