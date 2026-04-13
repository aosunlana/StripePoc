// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState } from 'react';
import type { AccountView } from '../../../models/views/AccountView';
import type { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';
import type { PaymentLifecycleEventView } from '../../../models/views/PaymentLifecycleEventView';

export enum PaymentStep {
    Identity = 1,
    Wallet = 2,
    Pricing = 3,
    Action = 4,
    Outcome = 5,
}

const usePaymentFlowPage = () => {
    const [currentStep, setCurrentStep] = useState<PaymentStep>(PaymentStep.Identity);
    const [currentAccount, setCurrentAccount] = useState<AccountView | null>(null);
    const [currentSelection, setCurrentSelection] = useState<ServiceSelectionView | null>(null);
    const [outcomeEvent, setOutcomeEvent] = useState<PaymentLifecycleEventView | null>(null);

    const onIdentityComplete = (account: AccountView) => {
        setCurrentAccount(account);
        setCurrentStep(PaymentStep.Wallet);
    };

    const onWalletComplete = () => setCurrentStep(PaymentStep.Pricing);

    const onPricingComplete = (selection: ServiceSelectionView) => {
        setCurrentSelection(selection);
        setCurrentStep(PaymentStep.Action);
    };

    const onActionComplete = (event: PaymentLifecycleEventView) => {
        setOutcomeEvent(event);
        setCurrentStep(PaymentStep.Outcome);
    };

    const onReset = () => {
        setCurrentAccount(null);
        setCurrentSelection(null);
        setOutcomeEvent(null);
        setCurrentStep(PaymentStep.Identity);
    };

    const onBack = (step: PaymentStep) => setCurrentStep(step);

    return {
        currentStep,
        currentAccount,
        currentSelection,
        outcomeEvent,
        onIdentityComplete,
        onWalletComplete,
        onPricingComplete,
        onActionComplete,
        onReset,
        onBack,
    };
};

export default usePaymentFlowPage;
