// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React, { useRef } from 'react';
import { Building2, ChevronRight } from 'lucide-react';
import StyleBase from '../../../bases/styles/StyleBase';
import DivisionBase from '../../../bases/divisions/DivisionBase';
import HeadingBase from '../../../bases/headings/HeadingBase';
import ParagraphBase from '../../../bases/paragraphs/ParagraphBase';
import LabelBase from '../../../bases/labels/LabelBase';
import SelectBase from '../../../bases/selects/SelectBase';
import OptionBase from '../../../bases/selects/OptionBase';
import ButtonBase from '../../../bases/buttons/ButtonBase';
import LoaderBase from '../../../bases/loaders/LoaderBase';
import { IdentityStepComponentStyles } from '../../../../models/components/foundations/identitySteps/identityStepComponentStyles';
import useIdentityStepComponent from './useIdentityStepComponent';

interface IdentityStepComponentProps {
    onNext: (account: import('../../../../models/views/AccountView').AccountView) => void;
}

const IdentityStepComponent: React.FC<IdentityStepComponentProps> = ({ onNext }) => {
    const styleRef = useRef<HTMLStyleElement>(null);

    const {
        businesses,
        accounts,
        selectedBusiness,
        selectedAccount,
        loading,
        error,
        onBusinessSelect,
        onAccountSelect,
        onVerifyIdentity,
    } = useIdentityStepComponent();

    const handleVerify = async () => {
        const success = await onVerifyIdentity();
        if (success && selectedAccount) onNext(selectedAccount);
    };

    return (
        <>
            <StyleBase componentRef={styleRef} styles={IdentityStepComponentStyles} />

            <DivisionBase className="card">
                <DivisionBase className="identity-step-header">
                    <DivisionBase className="identity-step-icon-wrap">
                        <Building2 size={24} color="var(--primary)" />
                    </DivisionBase>
                    <DivisionBase>
                        <HeadingBase className="identity-step-title" text="Identify Account" level={2} />
                        <ParagraphBase className="identity-step-subtitle" text="Select the business and account initiating the payment." />
                    </DivisionBase>
                </DivisionBase>

                <DivisionBase className="identity-step-container">
                    {error && <DivisionBase className="identity-step-error">{error}</DivisionBase>}

                    <DivisionBase className="identity-step-field-group">
                        <LabelBase className="identity-step-label" text="Business Partner" htmlFor="business-select" />
                        <SelectBase
                            id="business-select"
                            className="input"
                            value={selectedBusiness?.id || ''}
                            onChange={(e) => onBusinessSelect(e.target.value)}
                        >
                            <OptionBase value="" text="Select a business..." />
                            {businesses.map(b => (
                                <OptionBase key={b.id} value={b.id} text={b.name} />
                            ))}
                        </SelectBase>
                    </DivisionBase>

                    {selectedBusiness && (
                        <DivisionBase className="identity-step-field-group">
                            <LabelBase className="identity-step-label" text="Corporate Account" htmlFor="account-select" />
                            <SelectBase
                                id="account-select"
                                className="input"
                                value={selectedAccount?.id || ''}
                                onChange={(e) => onAccountSelect(e.target.value)}
                            >
                                <OptionBase value="" text="Select an account..." />
                                {accounts.map(a => (
                                    <OptionBase key={a.id} value={a.id} text={a.name} />
                                ))}
                            </SelectBase>
                        </DivisionBase>
                    )}
                </DivisionBase>

                <DivisionBase className="identity-step-footer">
                    <ButtonBase
                        id="verify-identity-btn"
                        className="btn btn-primary"
                        disabled={!selectedAccount || loading}
                        onClick={handleVerify}
                    >
                        {loading ? <LoaderBase size={18} /> : <>{<span>Verify Identity</span>}<ChevronRight size={18} /></>}
                    </ButtonBase>
                </DivisionBase>
            </DivisionBase>
        </>
    );
};

export default IdentityStepComponent;
