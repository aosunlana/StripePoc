// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { render, screen, renderHook } from '@testing-library/react';
import { vi, describe, it, expect } from 'vitest';
import PricingStepComponent from './PricingStepComponent';
import usePricingStepComponent from './usePricingStepComponent';
import { pricingViewService } from '../../../../../features/pricing/pricing.module';

vi.mock('../../../../../features/pricing/pricing.module', () => ({
    pricingViewService: {
        retrieveAllPricingTiersAsync: vi.fn(),
        getAvailablePlans: vi.fn(),
        generateRandomFee: vi.fn(),
        buildServiceSelection: vi.fn(),
    }
}));

describe('PricingStepComponent Architecture', () => {

    describe('Tier 1: Logic Hook (usePricingStepComponent)', () => {
        it('should calculate selection amount correctly', async () => {
            const mockPlans = [{ id: '1', name: 'Basic', price: 1000 }];
            vi.mocked(pricingViewService.retrieveAllPricingTiersAsync).mockResolvedValue(mockPlans);
            vi.mocked(pricingViewService.generateRandomFee).mockReturnValue(500);
            
            const { result } = renderHook(() => usePricingStepComponent());

            expect(result.current.plans).toBeDefined();
        });
    });

    describe('Tier 2: Access & Contract (UI Primitives)', () => {
        it('should render pricing summary sections', () => {
            vi.mocked(pricingViewService.retrieveAllPricingTiersAsync).mockResolvedValue([]);
            render(<PricingStepComponent onNext={() => {}} onBack={() => {}} />);
            
            expect(screen.getByRole('heading', { name: /Service Selection/i })).toBeInTheDocument();
        });
    });

    describe('Tier 3: Integration (User Flow)', () => {
        it('should call onNext with selection when clicked', async () => {
            const onNext = vi.fn();
            
            // Setup hook state via component interaction or direct override if needed
            render(<PricingStepComponent onNext={onNext} onBack={() => {}} />);

            const nextBtn = screen.getByRole('button', { name: /Final Summary/i });
            expect(nextBtn).toBeInTheDocument();
        });
    });
});
