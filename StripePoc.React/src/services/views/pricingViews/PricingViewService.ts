// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { PricingViewServiceExceptions } from './PricingViewService.Exceptions';
import type { IPricingViewService } from './IPricingViewService';
import { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';

export class PricingViewService extends PricingViewServiceExceptions implements IPricingViewService {
    constructor(
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async retrieveAllPricingTiersAsync(): Promise<any[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return this.getAvailablePlans();
        });
    }

    public getAvailablePlans(): any[] {
        return [
            { id: 'tier_1', name: 'Basic', price: 1500, currency: 'USD' },
            { id: 'tier_2', name: 'Premium', price: 4900, currency: 'USD' }
        ];
    }

    public generateRandomFee(): number {
        return Math.floor(Math.random() * 500);
    }

    public buildServiceSelection(planName: string, amount: number): ServiceSelectionView {
        return new ServiceSelectionView({
            serviceName: planName,
            amount: amount,
            currency: 'USD'
        });
    }
}
