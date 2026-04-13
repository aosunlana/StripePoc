// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../models/xeptions/Xeption';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import { PaymentMethodValidationException } from '../../../models/foundations/payments/exceptions/PaymentMethodValidationException';
import { PaymentMethodServiceValidations } from './PaymentMethodService.Validations';

export class PaymentMethodServiceExceptions extends PaymentMethodServiceValidations {
    protected async tryCatch<T>(loggingBroker: ILoggingBroker, returningFunction: () => Promise<T>): Promise<T> {
        try {
            return await returningFunction();
        } catch (error: unknown) {
            if (error instanceof Xeption) {
                const wrapper = new PaymentMethodValidationException("Payment method validation error occurred.", error);
                await loggingBroker.logErrorAsync(wrapper);
                throw wrapper;
            }
            throw (error instanceof Error) ? error : new Error(String(error));
        }
    }
}
