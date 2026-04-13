// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import { Elements } from '@stripe/react-stripe-js';
import { Wallet, Plus, CreditCard } from 'lucide-react';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import SpanBase from '../../../bases/spans/SpanBase';
import LoaderBase from '../../../bases/loaders/LoaderBase';
import CardFormComponent from '../cardForms/CardFormComponent';
import { WalletStepComponentStyles } from '../../../../models/components/foundations/walletSteps/walletStepComponentStyles';
import useWalletStepComponent from './useWalletStepComponent';
import { PaymentMethodView } from '../../../../models/views/PaymentMethodView';
import type { AccountView } from '../../../../models/views/AccountView';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY || 'pk_test_TYooMQauvdEDq54NiTphI7jx');

interface WalletStepComponentProps {
    account: AccountView;
    onNext: () => void;
    onBack: () => void;
}

const WalletStepComponent: React.FC<WalletStepComponentProps> = ({ account, onNext, onBack }) => {
    const styleRef = useRef<HTMLStyleElement>(null);
    const { paymentMethods, showAddCard, loading, error, onShowAddCard, onHideAddCard, onAddCard } = useWalletStepComponent(account.id);

    return (
        <>
            <StyleBase componentRef={styleRef} styles={WalletStepComponentStyles} />
            <DivisionBase className="card">
                <DivisionBase className="wallet-step-header">
                    <DivisionBase className="wallet-step-header-left">
                        <DivisionBase className="wallet-step-icon-wrap">
                            <Wallet size={24} color="var(--primary)" />
                        </DivisionBase>
                        <DivisionBase>
                            <HeadingBase className="wallet-step-title" text="Payment Wallet" level={2} />
                            <ParagraphBase className="wallet-step-subtitle" text={account.name} />
                        </DivisionBase>
                    </DivisionBase>
                    {!showAddCard && (
                        <ButtonBase className="btn btn-outline" onClick={onShowAddCard} id="add-card-btn">
                            <Plus size={18} />
                            <SpanBase text=" New Card" />
                        </ButtonBase>
                    )}
                </DivisionBase>

                {showAddCard ? (
                    <Elements stripe={stripePromise}>
                        <CardFormComponent accountId={account.id} onAdd={onAddCard} onCancel={onHideAddCard} />
                    </Elements>
                ) : (
                    <DivisionBase className="wallet-step-container">
                        {error && <ParagraphBase className="identity-step-error" text={error} />}
                        {loading ? (
                            <DivisionBase style={{ textAlign: 'center', padding: '2rem' }}>
                                <LoaderBase size={24} />
                            </DivisionBase>
                        ) : paymentMethods.length === 0 ? (
                            <DivisionBase className="wallet-step-empty">
                                <ParagraphBase text='No cards found. Click "New Card" to get started.' />
                            </DivisionBase>
                        ) : (
                            paymentMethods.map((m: PaymentMethodView, idx: number) => (
                                <DivisionBase key={m.id || idx} className="wallet-method-card">
                                    <DivisionBase className="wallet-method-info">
                                        <CreditCard size={20} color="var(--secondary)" />
                                        <DivisionBase>
                                            <SpanBase className="wallet-method-name" text={`${(m.brand || 'CARD').toUpperCase()} •••• ${m.last4 || '****'}`} />
                                            <ParagraphBase className="wallet-method-status" text="Active" />
                                        </DivisionBase>
                                    </DivisionBase>
                                    {m.isDefault && <SpanBase className="badge badge-success" text="Default" />}
                                </DivisionBase>
                            ))
                        )}
                    </DivisionBase>
                )}

                <DivisionBase className="wallet-step-footer">
                    <ButtonBase className="btn btn-outline" text="Back" onClick={onBack} />
                    <ButtonBase className="btn btn-primary" text="Confirm and Proceed" onClick={onNext} disabled={paymentMethods.length === 0} id="wallet-next-btn" />
                </DivisionBase>
            </DivisionBase>
        </>
    );
};

export default WalletStepComponent;
