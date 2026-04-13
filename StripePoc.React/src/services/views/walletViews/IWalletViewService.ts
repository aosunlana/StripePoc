// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { PaymentMethodView } from '../../../models/views/PaymentMethodView';
import type { Stripe, StripeElements } from '@stripe/stripe-js';

export interface IWalletViewService {
    retrievePaymentMethodsViewAsync(accountId: string): Promise<PaymentMethodView[]>;
    addCardViewAsync(accountId: string, stripe: Stripe | null, elements: StripeElements | null): Promise<PaymentMethodView>;
}
