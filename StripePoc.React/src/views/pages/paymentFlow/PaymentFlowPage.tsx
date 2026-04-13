// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import { Zap } from 'lucide-react';
import DivisionBase from '../../bases/divisions/DivisionBase';
import SpanBase from '../../bases/spans/SpanBase';
import IdentityStepComponent from '../../components/foundations/identitySteps/IdentityStepComponent';
import WalletStepComponent from '../../components/foundations/walletSteps/WalletStepComponent';
import PricingStepComponent from '../../components/foundations/pricingSteps/PricingStepComponent';
import ActionStepComponent from '../../components/foundations/actionSteps/ActionStepComponent';
import OutcomeViewComponent from '../../components/foundations/outcomeViews/OutcomeViewComponent';
import usePaymentFlowPage, { PaymentStep } from './usePaymentFlowPage';

const PaymentFlowPage: React.FC = () => {
    const {
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
    } = usePaymentFlowPage();

    return (
        <DivisionBase className="min-h-screen">
            <DivisionBase style={{ borderBottom: '1px solid var(--border)', background: 'white', padding: '1rem 2rem' }}>
                <DivisionBase style={{ maxWidth: '1200px', margin: '0 auto', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <DivisionBase style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                        <Zap size={24} color="var(--primary)" fill="var(--primary)" />
                        <SpanBase style={{ fontWeight: 800, fontSize: '1.25rem', letterSpacing: '-0.025em' }} text="EMERALD KILONOVA" />
                    </DivisionBase>
                </DivisionBase>
            </DivisionBase>

            <DivisionBase className="step-container">
                {currentStep < PaymentStep.Outcome && (
                    <DivisionBase className="stepper">
                        {([PaymentStep.Identity, PaymentStep.Wallet, PaymentStep.Pricing, PaymentStep.Action] as const).map((step, i) => (
                            <DivisionBase
                                key={step}
                                className={`step ${currentStep >= step ? 'active' : ''} ${currentStep > step ? 'completed' : ''}`}
                            >
                                <SpanBase text={String(i + 1)} />
                            </DivisionBase>
                        ))}
                    </DivisionBase>
                )}

                {currentStep === PaymentStep.Identity && (
                    <IdentityStepComponent onNext={onIdentityComplete} />
                )}

                {currentStep === PaymentStep.Wallet && currentAccount && (
                    <WalletStepComponent
                        account={currentAccount}
                        onNext={onWalletComplete}
                        onBack={() => onBack(PaymentStep.Identity)}
                    />
                )}

                {currentStep === PaymentStep.Pricing && (
                    <PricingStepComponent
                        onNext={onPricingComplete}
                        onBack={() => onBack(PaymentStep.Wallet)}
                    />
                )}

                {currentStep === PaymentStep.Action && currentAccount && currentSelection && (
                    <ActionStepComponent
                        account={currentAccount}
                        selection={currentSelection}
                        onSuccess={onActionComplete}
                        onBack={() => onBack(PaymentStep.Pricing)}
                    />
                )}

                {currentStep === PaymentStep.Outcome && outcomeEvent && (
                    <OutcomeViewComponent event={outcomeEvent} onReset={onReset} />
                )}
            </DivisionBase>
        </DivisionBase>
    );
};

export default PaymentFlowPage;
