// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { CheckCircle, AlertCircle, ExternalLink, RefreshCw } from 'lucide-react';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import SpanBase from '../../../bases/spans/SpanBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import AnchorBase from '../../../bases/anchors/AnchorBase';
import { OutcomeViewComponentStyles } from '../../../../models/components/foundations/outcomeViews/outcomeViewComponentStyles';
import type { PaymentLifecycleEventView } from '../../../../models/views/PaymentLifecycleEventView';

interface OutcomeViewComponentProps {
    event: PaymentLifecycleEventView;
    onReset: () => void;
}

const OutcomeViewComponent: React.FC<OutcomeViewComponentProps> = ({ event, onReset }) => {
    const styleRef = useRef<HTMLStyleElement>(null);
    const rawEvent = event as any;
    const statusOrFallback = event.status || rawEvent.Status || (event.isValid || rawEvent.IsValid ? 'succeeded' : 'failed');
    const isSuccess = statusOrFallback === 'succeeded' || statusOrFallback === 'processing';
    const checkoutUrl = event.checkoutUrl || rawEvent.CheckoutUrl;
    const externalRef = event.externalReferenceId || rawEvent.ExternalReferenceId || 'N/A';
    const message = event.message || rawEvent.Message;
    return (
        <>
            <StyleBase componentRef={styleRef} styles={OutcomeViewComponentStyles} />

            <DivisionBase className="card outcome-card">
                <DivisionBase className="outcome-icon-wrap">
                    {isSuccess ? (
                        <CheckCircle size={64} color="#10b981" />
                    ) : (
                        <AlertCircle size={64} color="#ef4444" />
                    )}
                </DivisionBase>

                <HeadingBase
                    className="outcome-title"
                    text={isSuccess ? "Transaction Authorized" : "Transaction Failed"}
                    level={2}
                />

                <ParagraphBase
                    className="outcome-message"
                    text={message || (isSuccess ? "Your payment instruction has been successfully processed." : "The transaction was declined by the provider.")}
                />

                <DivisionBase className="outcome-details">
                    <DivisionBase className="outcome-detail-row">
                        <SpanBase className="outcome-detail-label" text="Reference ID" />
                        <SpanBase className="outcome-detail-value" text={externalRef} />
                    </DivisionBase>
                    <DivisionBase className="outcome-detail-row">
                        <SpanBase className="outcome-detail-label" text="Status" />
                        <SpanBase className={`badge ${isSuccess ? 'badge-success' : 'badge-pending'}`} text={statusOrFallback.toUpperCase()} />
                    </DivisionBase>
                </DivisionBase>

                <DivisionBase className="outcome-actions">
                    {checkoutUrl && (
                        <AnchorBase
                            className="btn btn-outline"
                            href={checkoutUrl}
                            target="_blank"
                            rel="noopener noreferrer"
                            id="view-receipt-link"
                        >
                            <ExternalLink size={18} />
                            <SpanBase text=" View Formal Receipt" />
                        </AnchorBase>
                    )}

                    <ButtonBase className="btn btn-primary" onClick={onReset} id="start-new-btn">
                        <RefreshCw size={18} />
                        <SpanBase text=" Start New Process" />
                    </ButtonBase>
                </DivisionBase>
            </DivisionBase>
        </>
    );
};

export default OutcomeViewComponent;
