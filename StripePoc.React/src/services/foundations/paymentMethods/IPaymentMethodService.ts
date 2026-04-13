// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { PaymentMethodView } from '../../../models/views/PaymentMethodView';

export interface IPaymentMethodService {
    retrieveAllPaymentMethodsAsync(accountId: string): Promise<PaymentMethodView[]>;
    initiateCardSetupAsync(accountId: string): Promise<string>;
    finaliseCardSetupAsync(accountId: string, stripePaymentMethodId: string): Promise<PaymentMethodView>;
    addCardAsync(accountId: string, stripePaymentMethodId: string): Promise<PaymentMethodView>;
}
