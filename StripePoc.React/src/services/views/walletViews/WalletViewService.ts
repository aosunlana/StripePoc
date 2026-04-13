// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IPaymentMethodService } from '../../foundations/paymentMethods/IPaymentMethodService';
import type { IWalletViewService } from './IWalletViewService';
import { PaymentMethodView } from '../../../models/views/PaymentMethodView';
import { WalletViewServiceExceptions } from './WalletViewService.Exceptions';
import type { Stripe, StripeElements } from '@stripe/stripe-js';

export class WalletViewService extends WalletViewServiceExceptions implements IWalletViewService {
    constructor(
        private readonly paymentMethodService: IPaymentMethodService,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async retrievePaymentMethodsViewAsync(accountId: string): Promise<PaymentMethodView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.paymentMethodService.retrieveAllPaymentMethodsAsync(accountId);
        });
    }

    public async addCardViewAsync(accountId: string, stripe: Stripe | null, elements: StripeElements | null): Promise<PaymentMethodView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            if (!stripe || !elements) throw new Error("Stripe or Elements not initialized.");
            
            const cardElement = elements.getElement('card');
            if (!cardElement) throw new Error("Card element not found.");

            const stripeMethod = await stripe.createPaymentMethod({
                type: 'card',
                card: cardElement,
            });

            if (stripeMethod.error) throw new Error(stripeMethod.error.message);

            return await this.paymentMethodService.addCardAsync(accountId, stripeMethod.paymentMethod.id);
        });
    }
}
