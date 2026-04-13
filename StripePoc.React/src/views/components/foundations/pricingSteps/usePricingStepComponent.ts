// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { useState, useEffect } from 'react';
import { pricingViewService } from '../../../../../features/pricing/pricing.module';
import type { ServiceSelectionView } from '../../../../models/views/ServiceSelectionView';

const usePricingStepComponent = () => {
    const [plans, setPlans] = useState<any[]>([]);
    const [selection, setSelection] = useState<ServiceSelectionView | null>(null);
    const [fee, setFee] = useState<number>(0);

    useEffect(() => {
        pricingViewService.retrieveAllPricingTiersAsync()
            .then((data: any[]) => {
                setPlans(data);
                setFee(pricingViewService.generateRandomFee());
            });
    }, []);

    const onPlanSelect = (plan: any) => {
        const newSelection = pricingViewService.buildServiceSelection(plan.name, plan.price);
        setSelection(newSelection);
    };

    const isPlanSelected = (plan: any) => selection?.serviceName === plan.name;

    return {
        plans,
        selection,
        fee,
        onPlanSelect,
        isPlanSelected,
    };
};

export default usePricingStepComponent;
