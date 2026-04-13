// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';

export interface IPricingViewService {
    retrieveAllPricingTiersAsync(): Promise<any[]>;
    getAvailablePlans(): any[];
    generateRandomFee(): number;
    buildServiceSelection(planName: string, amount: number): ServiceSelectionView;
}
