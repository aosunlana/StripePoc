// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, fireEvent, renderHook, waitFor, act } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import WalletStepComponent from './WalletStepComponent';
import useWalletStepComponent from './useWalletStepComponent';
import { walletViewService } from '../../../../../features/wallet/wallet.module';
import { PaymentMethodView } from '../../../../models/views/PaymentMethodView';

vi.mock('../../../../../features/wallet/wallet.module', () => ({
    walletViewService: {
        retrievePaymentMethodsViewAsync: vi.fn(),
        addCardViewAsync: vi.fn(),
    }
}));

const mockAccount = { id: 'acc1', name: 'Test Account' };

describe('WalletStepComponent Architecture', () => {

    describe('Tier 1: Logic Hook (useWalletStepComponent)', () => {
        it('should retrieve payment methods on mount', async () => {
            const mockMethods: PaymentMethodView[] = [
                { id: 'pm1', type: 'card', brand: 'visa', last4: '4242', expiryMonth: 12, expiryYear: 2025, isDefault: true } as PaymentMethodView
            ];
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockResolvedValue(mockMethods);

            const { result } = renderHook(() => useWalletStepComponent(mockAccount.id));

            await waitFor(() => {
                expect(result.current.paymentMethods).toEqual(mockMethods);
            });
        });

        it('should toggle "Add Card" state', () => {
            const { result } = renderHook(() => useWalletStepComponent(mockAccount.id));

            act(() => {
                result.current.onShowAddCard();
            });
            expect(result.current.showAddCard).toBe(true);

            act(() => {
                result.current.onHideAddCard();
            });
            expect(result.current.showAddCard).toBe(false);
        });
    });

    describe('Tier 2: Access & Contract (UI Primitives)', () => {
        it('should render loading state initially', () => {
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockReturnValue(new Promise(() => {}));
            render(<WalletStepComponent account={mockAccount} onNext={() => {}} onBack={() => {}} />);
            
            expect(screen.getByRole('heading', { name: /Payment Wallet/i })).toBeInTheDocument();
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should display list of payment methods', async () => {
            const mockMethods: PaymentMethodView[] = [
                { id: 'pm1', type: 'card', brand: 'visa', last4: '4242', expiryMonth: 12, expiryYear: 2025, isDefault: true } as PaymentMethodView
            ];
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockResolvedValue(mockMethods);

            render(<WalletStepComponent account={mockAccount} onNext={() => {}} onBack={() => {}} />);

            await waitFor(() => {
                expect(screen.getByText(/VISA •••• 4242/i)).toBeInTheDocument();
            });
        });

        it('should show empty state if no methods found', async () => {
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockResolvedValue([]);

            render(<WalletStepComponent account={mockAccount} onNext={() => {}} onBack={() => {}} />);

            await waitFor(() => {
                expect(screen.getByText(/No cards found/i)).toBeInTheDocument();
            });
        });

        it('should enable "Confirm" only when methods exist', async () => {
            const mockMethods: PaymentMethodView[] = [
                { id: 'pm1', type: 'card', brand: 'visa', last4: '4242', expiryMonth: 12, expiryYear: 2025, isDefault: true } as PaymentMethodView
            ];
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockResolvedValue(mockMethods);

            render(<WalletStepComponent account={mockAccount} onNext={() => {}} onBack={() => {}} />);

            await waitFor(() => {
                const nextBtn = screen.getByRole('button', { name: /Confirm and Proceed/i });
                expect(nextBtn).not.toBeDisabled();
            });
        });

        it('should navigate back correctly', () => {
            const onBack = vi.fn();
            vi.mocked(walletViewService.retrievePaymentMethodsViewAsync).mockResolvedValue([]);
            
            render(<WalletStepComponent account={mockAccount} onNext={() => {}} onBack={onBack} />);

            const backBtn = screen.getByRole('button', { name: /Back/i });
            fireEvent.click(backBtn);

            expect(onBack).toHaveBeenCalled();
        });
    });
});
