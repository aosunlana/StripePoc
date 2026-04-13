// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IApiBroker } from '../../../brokers/apis/IApiBroker';
import type { IActionService } from './IActionService';
import { PaymentLifecycleEventView } from '../../../models/views/PaymentLifecycleEventView';
import { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';
import { ActionServiceExceptions } from './ActionService.Exceptions';

export class ActionService extends ActionServiceExceptions implements IActionService {
    constructor(
        private readonly apiBroker: IApiBroker,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async directChargeAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateId(accountId, "accountId");
            this.validateServiceSelection(selection);

            return await this.apiBroker.createSubscriptionAsync({
                accountId,
                serviceName: selection.serviceName,
                amount: selection.amount,
                currency: selection.currency,
                stripePriceId: ''
            });
        });
    }

    public async requestSubscriptionQuoteAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateId(accountId, "accountId");
            this.validateServiceSelection(selection);

            return await this.apiBroker.createSubscriptionQuoteAsync({
                accountId,
                serviceName: selection.serviceName,
                amount: selection.amount,
                currency: selection.currency,
                stripePriceId: ''
            });
        });
    }

    public async requestOneTimeQuoteAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateId(accountId, "accountId");
            this.validateServiceSelection(selection);

            return await this.apiBroker.processOneTimeQuotePaymentAsync({
                accountId,
                amount: selection.amount,
                currency: selection.currency,
                status: 0
            });
        });
    }
}
