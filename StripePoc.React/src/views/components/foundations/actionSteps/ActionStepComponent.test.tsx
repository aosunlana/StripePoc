// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, fireEvent, renderHook, waitFor, act } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import ActionStepComponent from './ActionStepComponent';
import useActionStepComponent from './useActionStepComponent';
import { actionViewService } from '../../../../../features/action/action.module';
import { AccountView } from '../../../../models/views/AccountView';
import { ServiceSelectionView } from '../../../../models/views/ServiceSelectionView';

vi.mock('../../../../../features/action/action.module', () => ({
    actionViewService: {
        directChargeViewAsync: vi.fn(),
        requestSubscriptionQuoteViewAsync: vi.fn(),
        requestOneTimeQuoteViewAsync: vi.fn(),
    }
}));

const mockAccount: AccountView = { id: 'acc1', name: 'Test Account' };
const mockSelection: ServiceSelectionView = { serviceName: 'Basic', amount: 1500, currency: 'USD' };

const mockEvent = { 
    isValid: true, 
    eventType: 'sub.created', 
    status: 'succeeded', 
    externalReferenceId: 'evt1', 
    amount: 1500, 
    currency: 'USD', 
    checkoutUrl: '',
    message: 'Success', 
    occurredAt: 'now' 
};

describe('ActionStepComponent Architecture', () => {

    beforeEach(() => {
        vi.clearAllMocks();
    });

    describe('Tier 1: Logic Hook (useActionStepComponent)', () => {
        it('should trigger direct charge and return result', async () => {
            vi.mocked(actionViewService.directChargeViewAsync).mockResolvedValue(mockEvent);

            const { result } = renderHook(() => useActionStepComponent(mockAccount, mockSelection));

            let finalResult;
            await act(async () => {
                finalResult = await result.current.onDirectCharge();
            });

            expect(finalResult).toEqual(mockEvent);
            expect(actionViewService.directChargeViewAsync).toHaveBeenCalledWith('acc1', mockSelection);
        });
    });

    describe('Tier 2: Access & Contract (UI Primitives)', () => {
        it('should render all authorization options', () => {
            render(<ActionStepComponent account={mockAccount} selection={mockSelection} onSuccess={() => {}} onBack={() => {}} />);
            
            expect(screen.getByText(/Direct Auto-Charge/i)).toBeInTheDocument();
            expect(screen.getByRole('button', { name: /Legal Approval/i })).toBeInTheDocument();
            expect(screen.getByRole('button', { name: /One-time Quote/i })).toBeInTheDocument();
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should navigate to success outcome on direct charge success', async () => {
            const onSuccess = vi.fn();
            vi.mocked(actionViewService.directChargeViewAsync).mockResolvedValue(mockEvent);

            render(<ActionStepComponent account={mockAccount} selection={mockSelection} onSuccess={onSuccess} onBack={() => {}} />);

            const directCard = screen.getByText(/Direct Auto-Charge/i);
            fireEvent.click(directCard);

            await waitFor(() => {
                expect(onSuccess).toHaveBeenCalledWith(mockEvent);
            });
        });
    });
});
