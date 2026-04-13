// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IApiBroker } from '../../../brokers/apis/IApiBroker';
import type { IPaymentMethodService } from './IPaymentMethodService';
import { PaymentMethodView } from '../../../models/views/PaymentMethodView';
import { PaymentMethodServiceExceptions } from './PaymentMethodService.Exceptions';

export class PaymentMethodService extends PaymentMethodServiceExceptions implements IPaymentMethodService {
    constructor(
        private readonly apiBroker: IApiBroker,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async initiateCardSetupAsync(accountId: string): Promise<string> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateAccountId(accountId);
            const result = await this.apiBroker.initiateCardSetupAsync(accountId);
            return result.clientSecret;
        });
    }

    public async addCardAsync(accountId: string, stripePaymentMethodId: string): Promise<PaymentMethodView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateAccountId(accountId);
            this.validatePaymentMethodId(stripePaymentMethodId);
            return await this.apiBroker.finaliseCardSetupAsync(accountId, stripePaymentMethodId);
        });
    }

    public async finaliseCardSetupAsync(accountId: string, stripePaymentMethodId: string): Promise<PaymentMethodView> {
        return await this.addCardAsync(accountId, stripePaymentMethodId);
    }

    public async retrieveAllPaymentMethodsAsync(accountId: string): Promise<PaymentMethodView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateAccountId(accountId);
            return await this.apiBroker.getPaymentMethodsAsync(accountId);
        });
    }
}
