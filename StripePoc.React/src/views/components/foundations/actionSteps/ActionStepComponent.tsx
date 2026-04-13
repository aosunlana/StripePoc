// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { ShieldCheck, Zap, Mail, ArrowLeft, Link as LinkIcon } from 'lucide-react';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import ClickableDivisionBase from '../../../bases/divisions/ClickableDivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import SpanBase from '../../../bases/spans/SpanBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import LoaderBase from '../../../bases/loaders/LoaderBase';
import { ActionStepComponentStyles } from '../../../../models/components/foundations/actionSteps/actionStepComponentStyles';
import useActionStepComponent from './useActionStepComponent';
import type { AccountView } from '../../../../models/views/AccountView';
import type { ServiceSelectionView } from '../../../../models/views/ServiceSelectionView';
import type { PaymentLifecycleEventView } from '../../../../models/views/PaymentLifecycleEventView';

interface ActionStepComponentProps {
    account: AccountView;
    selection: ServiceSelectionView;
    onSuccess: (event: PaymentLifecycleEventView) => void;
    onBack: () => void;
}

const ActionStepComponent: React.FC<ActionStepComponentProps> = ({ account, selection, onSuccess, onBack }) => {
    const styleRef = useRef<HTMLStyleElement>(null);
    const { loading, error, onDirectCharge, onRequestSubscriptionQuote, onRequestOneTimeQuote } = useActionStepComponent(account, selection);

    const handleDirectCharge = async () => {
        const result = await onDirectCharge();
        if (result) onSuccess(result);
    };

    const handleSubQuote = async () => {
        const result = await onRequestSubscriptionQuote();
        if (result) onSuccess(result);
    };

    const handleOneTimeQuote = async () => {
        const result = await onRequestOneTimeQuote();
        if (result) onSuccess(result);
    };

    return (
        <>
            <StyleBase componentRef={styleRef} styles={ActionStepComponentStyles} />

            <DivisionBase className="card">
                <DivisionBase className="action-step-header">
                    <DivisionBase className="action-step-icon-wrap">
                        <ShieldCheck size={24} color="var(--primary)" />
                    </DivisionBase>
                    <DivisionBase>
                        <HeadingBase className="action-step-title" text="Final Authorization" level={2} />
                        <ParagraphBase className="action-step-subtitle" text="Review the transaction and choose an authorization path." />
                    </DivisionBase>
                </DivisionBase>

                <DivisionBase className="action-step-summary">
                    <DivisionBase>
                        <ParagraphBase className="action-step-summary-label" text="Service" />
                        <ParagraphBase className="action-step-summary-value" text={selection.serviceName} />
                    </DivisionBase>
                    <DivisionBase style={{ textAlign: 'right' }}>
                        <ParagraphBase className="action-step-summary-label" text="Weekly Rate" />
                        <ParagraphBase className="action-step-summary-value" text={`$${(selection.amount / 100).toFixed(2)}`} />
                    </DivisionBase>
                </DivisionBase>

                {error && <DivisionBase className="action-step-error">{error}</DivisionBase>}

                <DivisionBase className="action-step-grid">
                    <ClickableDivisionBase className="action-direct-card" onClick={handleDirectCharge}>
                        <DivisionBase style={{ padding: '0.75rem', background: '#ecfdf5', borderRadius: 'var(--radius)', display: 'flex', alignItems: 'center' }}>
                            <Zap size={24} color="#059669" />
                        </DivisionBase>
                        <DivisionBase style={{ flex: 1 }}>
                            <DivisionBase style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                <HeadingBase text="Direct Auto-Charge" level={4} style={{ margin: 0, fontWeight: 700 }} />
                                {loading === 'direct' && <LoaderBase size={18} />}
                            </DivisionBase>
                            <ParagraphBase text="Charge the corporate card on file immediately." style={{ fontSize: '0.875rem', color: 'var(--secondary)' }} />
                        </DivisionBase>
                    </ClickableDivisionBase>

                    <DivisionBase className="action-step-quotes-grid">
                        <ButtonBase className="btn btn-outline" onClick={handleSubQuote} disabled={!!loading}
                            style={{ flexDirection: 'column', height: 'auto', padding: '1.5rem', gap: '0.75rem' }}
                            id="sub-quote-btn">
                            <DivisionBase style={{ padding: '0.5rem', background: '#eff6ff', borderRadius: '50%' }}>
                                <Mail size={20} color="#2563eb" />
                            </DivisionBase>
                            <DivisionBase className="action-quote-btn-inner">
                                <SpanBase text="Legal Approval (Sub)" style={{ fontWeight: 700 }} />
                                <SpanBase text="Send Quote for Review" style={{ fontSize: '0.75rem', opacity: 0.7 }} />
                            </DivisionBase>
                            {loading === 'quote-sub' && <LoaderBase size={16} />}
                        </ButtonBase>

                        <ButtonBase className="btn btn-outline" onClick={handleOneTimeQuote} disabled={!!loading}
                            style={{ flexDirection: 'column', height: 'auto', padding: '1.5rem', gap: '0.75rem' }}
                            id="onetime-quote-btn">
                            <DivisionBase style={{ padding: '0.5rem', background: '#fef3c7', borderRadius: '50%' }}>
                                <LinkIcon size={20} color="#d97706" />
                            </DivisionBase>
                            <DivisionBase className="action-quote-btn-inner">
                                <SpanBase text="One-time Quote" style={{ fontWeight: 700 }} />
                                <SpanBase text="Professional Authorization" style={{ fontSize: '0.75rem', opacity: 0.7 }} />
                            </DivisionBase>
                            {loading === 'quote-one' && <LoaderBase size={16} />}
                        </ButtonBase>
                    </DivisionBase>
                </DivisionBase>

                <DivisionBase className="action-step-footer">
                    <ButtonBase className="btn btn-outline" onClick={onBack}>
                        <ArrowLeft size={18} />
                        <SpanBase text=" Back to Plans" />
                    </ButtonBase>
                </DivisionBase>
            </DivisionBase>
        </>
    );
};

export default ActionStepComponent;
