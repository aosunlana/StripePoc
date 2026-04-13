// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, fireEvent, renderHook, waitFor, act } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import IdentityStepComponent from './IdentityStepComponent';
import useIdentityStepComponent from './useIdentityStepComponent';
import { identityViewService } from '../../../../../features/identity/identity.module';

// Mock the identity view service
vi.mock('../../../../../features/identity/identity.module', () => ({
    identityViewService: {
        retrieveAllBusinessesViewAsync: vi.fn(),
        retrieveAccountsByBusinessIdViewAsync: vi.fn(),
        verifyIdentityViewAsync: vi.fn(),
    }
}));

const mockBusinesses = [
    { id: 'b1', name: 'Business One' }
];

const mockAccounts = [
    { id: 'a1', name: 'Account One' }
];

describe('IdentityStepComponent Architecture', () => {

    beforeEach(() => {
        vi.clearAllMocks();
    });

    describe('Tier 1: Logic Hook (useIdentityStepComponent)', () => {
        it('should load businesses on mount', async () => {
            vi.mocked(identityViewService.retrieveAllBusinessesViewAsync).mockResolvedValue(mockBusinesses);

            const { result } = renderHook(() => useIdentityStepComponent());

            expect(result.current.loading).toBe(true);
            await waitFor(() => expect(result.current.loading).toBe(false));
            expect(result.current.businesses).toEqual(mockBusinesses);
        });

        it('should load accounts when a business is selected', async () => {
            vi.mocked(identityViewService.retrieveAllBusinessesViewAsync).mockResolvedValue(mockBusinesses);
            vi.mocked(identityViewService.retrieveAccountsByBusinessIdViewAsync).mockResolvedValue(mockAccounts);

            const { result } = renderHook(() => useIdentityStepComponent());
            
            await waitFor(() => expect(result.current.businesses).toHaveLength(1));

            await act(async () => {
                await result.current.onBusinessSelect('b1');
            });

            expect(result.current.selectedBusiness?.id).toBe('b1');
            expect(result.current.accounts).toEqual(mockAccounts);
        });
    });

    describe('Tier 2: Access & Contract (UI Primitives)', () => {
        it('should render semantic headers and labeled inputs', async () => {
            vi.mocked(identityViewService.retrieveAllBusinessesViewAsync).mockResolvedValue(mockBusinesses);
            
            render(<IdentityStepComponent onNext={() => {}} />);

            // Wait for mounting fetch to settle
            await waitFor(() => expect(screen.queryByRole('progressbar')).not.toBeInTheDocument());

            // Check HeadingBase contract
            expect(screen.getByRole('heading', { level: 2 })).toHaveTextContent('Identify Account');

            // Check LabelBase and SelectBase contract
            expect(screen.getByLabelText(/Business Partner/i)).toBeInTheDocument();
        });

        it('should have a primary button with the correct ID for automation', async () => {
            vi.mocked(identityViewService.retrieveAllBusinessesViewAsync).mockResolvedValue(mockBusinesses);
            render(<IdentityStepComponent onNext={() => {}} />);
            
            await waitFor(() => expect(screen.getByRole('button')).toBeInTheDocument());

            const button = screen.getByRole('button');
            expect(button).toHaveAttribute('id', 'verify-identity-btn');
            expect(button).toHaveClass('btn-primary');
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should complete the identity step successfully', async () => {
            const onNext = vi.fn();
            vi.mocked(identityViewService.retrieveAllBusinessesViewAsync).mockResolvedValue(mockBusinesses);
            vi.mocked(identityViewService.retrieveAccountsByBusinessIdViewAsync).mockResolvedValue(mockAccounts);
            vi.mocked(identityViewService.verifyIdentityViewAsync).mockResolvedValue({ stripeCustomerId: 'cus_123' });

            render(<IdentityStepComponent onNext={onNext} />);

            // 1. Select Business
            await waitFor(() => screen.getByLabelText(/Business Partner/i));
            fireEvent.change(screen.getByLabelText(/Business Partner/i), { target: { value: 'b1' } });

            // 2. Select Account (should appear after business selection)
            await waitFor(() => screen.getByLabelText(/Corporate Account/i));
            fireEvent.change(screen.getByLabelText(/Corporate Account/i), { target: { value: 'a1' } });

            // 3. Verify
            const verifyBtn = screen.getByRole('button', { name: /Verify Identity/i });
            fireEvent.click(verifyBtn);

            await waitFor(() => expect(onNext).toHaveBeenCalledWith(mockAccounts[0]));
        });
    });
});
