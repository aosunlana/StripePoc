// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { InvalidPaymentMethodException } from '../../../models/foundations/payments/exceptions/InvalidPaymentMethodException';

export class PaymentMethodServiceValidations {
    protected validateAccountId(accountId: string): void {
        this.validateId(accountId, "accountId");
    }

    protected validatePaymentMethodId(paymentMethodId: string): void {
        this.validateId(paymentMethodId, "paymentMethodId");
    }

    private validateId(id: string, name: string): void {
        if (!id) {
            const invalidPaymentMethodException = new InvalidPaymentMethodException("Invalid payment method, fix errors and try again.");
            invalidPaymentMethodException.upsertDataList(name, `${name} is required`);
            invalidPaymentMethodException.throwIfContainsErrors();
        }
    }
}
