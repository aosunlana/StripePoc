// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, fireEvent } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import OutcomeViewComponent from './OutcomeViewComponent';
import { PaymentLifecycleEventView } from '../../../../models/views/PaymentLifecycleEventView';

const mockEvent: PaymentLifecycleEventView = {
    isValid: true,
    eventType: 'sub.created',
    externalReferenceId: 'tx_123',
    status: 'succeeded',
    amount: 1500,
    currency: 'USD',
    message: 'Payment received',
    checkoutUrl: 'https://receipt.url',
    occurredAt: 'now'
};

describe('OutcomeViewComponent Architecture', () => {

    describe('Tier 1 & 2: Stateless UI & Contract', () => {
        it('should render success state correctly', () => {
            render(<OutcomeViewComponent event={mockEvent} onReset={() => {}} />);
            
            expect(screen.getByText(/Transaction Authorized/i)).toBeInTheDocument();
            expect(screen.getByText('tx_123')).toBeInTheDocument();
            expect(screen.getByText('SUCCEEDED')).toBeInTheDocument();
        });

        it('should render failure state correctly', () => {
            const failEvent = { ...mockEvent, status: 'failed' };
            render(<OutcomeViewComponent event={failEvent} onReset={() => {}} />);
            
            expect(screen.getByText(/Transaction Failed/i)).toBeInTheDocument();
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should call onReset when "Start New Process" is clicked', () => {
            const onReset = vi.fn();
            render(<OutcomeViewComponent event={mockEvent} onReset={onReset} />);
            
            const btn = screen.getByRole('button', { name: /Start New Process/i });
            fireEvent.click(btn);

            expect(onReset).toHaveBeenCalled();
        });

        it('should link to the receipt correctly', () => {
            render(<OutcomeViewComponent event={mockEvent} onReset={() => {}} />);
            
            const link = screen.getByRole('link', { name: /View Formal Receipt/i });
            expect(link).toHaveAttribute('href', 'https://receipt.url');
            expect(link).toHaveAttribute('target', '_blank');
        });
    });
});
