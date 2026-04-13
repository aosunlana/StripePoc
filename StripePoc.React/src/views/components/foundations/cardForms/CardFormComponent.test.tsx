// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, fireEvent, renderHook, waitFor, act } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import CardFormComponent from './CardFormComponent';
import useCardFormComponent from './useCardFormComponent';

describe('CardFormComponent Architecture', () => {

    describe('Tier 1: Logic Hook (useCardFormComponent)', () => {
        it('should call onAdd during submission', async () => {
            const onAdd = vi.fn().mockResolvedValue(true);
            const { result } = renderHook(() => useCardFormComponent('acc1', onAdd));

            await act(async () => {
                await result.current.onSubmit({ preventDefault: () => {} } as React.FormEvent);
            });

            expect(onAdd).toHaveBeenCalled();
            expect(result.current.processing).toBe(false);
        });
    });

    describe('Tier 2: Access & Contract (UI Primitives)', () => {
        it('should render form structure with accessible labels', () => {
            render(<CardFormComponent accountId="acc1" onAdd={async () => true} onCancel={() => {}} />);
            
            expect(screen.getByRole('form')).toBeInTheDocument();
            expect(screen.getByRole('button', { name: /Save Payment Method/i })).toBeInTheDocument();
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should show error message if submission fails', async () => {
            const onAdd = vi.fn().mockRejectedValue(new Error('Stripe Error'));
            render(<CardFormComponent accountId="acc1" onAdd={onAdd} onCancel={() => {}} />);

            const submitBtn = screen.getByRole('button', { name: /Save Payment Method/i });
            fireEvent.click(submitBtn);

            await waitFor(() => {
                expect(screen.getByText(/Stripe Error/i)).toBeInTheDocument();
            });
        });
    });
});
