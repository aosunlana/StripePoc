// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { Package, ChevronRight, ArrowLeft, Info } from 'lucide-react';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import SpanBase from '../../../bases/spans/SpanBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import { PricingStepComponentStyles } from '../../../../models/components/foundations/pricingSteps/pricingStepComponentStyles';
import { PlanCardComponentStyles } from '../../../../models/components/foundations/planCards/planCardComponentStyles';
import usePricingStepComponent from './usePricingStepComponent';
import type { ServiceSelectionView } from '../../../../models/views/ServiceSelectionView';

interface PricingStepComponentProps {
    onNext: (selection: ServiceSelectionView) => void;
    onBack: () => void;
}

const PricingStepComponent: React.FC<PricingStepComponentProps> = ({ onNext, onBack }) => {
    const styleRef = useRef<HTMLStyleElement>(null);
    const { plans, selection, fee, onPlanSelect, isPlanSelected } = usePricingStepComponent();

    return (
        <>
            <StyleBase componentRef={styleRef} styles={PricingStepComponentStyles} />
            <StyleBase styles={PlanCardComponentStyles} />

            <DivisionBase className="card">
                <DivisionBase className="pricing-step-header">
                    <DivisionBase className="pricing-step-icon-wrap">
                        <Package size={24} color="var(--primary)" />
                    </DivisionBase>
                    <DivisionBase>
                        <HeadingBase className="pricing-step-title" text="Service Selection" level={2} />
                        <ParagraphBase className="pricing-step-subtitle" text="Choose a plan that fits your corporate needs." />
                    </DivisionBase>
                </DivisionBase>

                <DivisionBase className="pricing-step-plans">
                    {plans.map((plan: any) => (
                        <DivisionBase
                            key={plan.id}
                            className={`pricing-plan ${isPlanSelected(plan) ? 'selected' : ''}`}
                            onClick={() => onPlanSelect(plan)}
                        >
                            <DivisionBase>
                                <ParagraphBase className="plan-card-name" text={plan.name} />
                                <ParagraphBase className="plan-card-description" text={plan.description || "Enterprise-grade solution."} />
                            </DivisionBase>
                            <DivisionBase>
                                <ParagraphBase className="plan-card-price" text={`$${(plan.price / 100).toFixed(2)}`} />
                                <ParagraphBase className="plan-card-period" text="per week" />
                            </DivisionBase>
                        </DivisionBase>
                    ))}
                </DivisionBase>

                {selection && (
                    <DivisionBase className="pricing-step-summary">
                        <DivisionBase className="pricing-step-summary-title">
                            <Info size={16} />
                            <SpanBase text="Estimated Weekly Total" />
                        </DivisionBase>
                        <DivisionBase className="pricing-step-summary-row">
                            <SpanBase text="Base Subscription:" />
                            <SpanBase text={`$${(selection.amount / 100).toFixed(2)}`} />
                        </DivisionBase>
                        <DivisionBase className="pricing-step-summary-row">
                            <SpanBase text="Dynamic Service Fee:" />
                            <SpanBase text={`+$${(fee / 100).toFixed(2)}`} />
                        </DivisionBase>
                        <DivisionBase className="pricing-step-summary-total">
                            <SpanBase text="Weekly Estimate:" />
                            <SpanBase style={{ color: 'var(--primary)' }} text={`$${((selection.amount + fee) / 100).toFixed(2)}`} />
                        </DivisionBase>
                    </DivisionBase>
                )}

                <DivisionBase className="pricing-step-footer">
                    <ButtonBase className="btn btn-outline" onClick={onBack}>
                        <ArrowLeft size={18} />
                        <SpanBase text=" Back" />
                    </ButtonBase>
                    <ButtonBase 
                        className="btn btn-primary" 
                        disabled={!selection}
                        onClick={() => selection && onNext(selection)} 
                        id="pricing-next-btn"
                    >
                        <SpanBase text="Final Summary" />
                        <ChevronRight size={18} />
                    </ButtonBase>
                </DivisionBase>
            </DivisionBase>
        </>
    );
};

export default PricingStepComponent;
