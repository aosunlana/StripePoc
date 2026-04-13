// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../models/xeptions/Xeption';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import { IdentityViewValidationException } from '../../../models/views/identityViews/exceptions/IdentityViewValidationException';

export class IdentityViewServiceExceptions {
    protected async tryCatch<T>(loggingBroker: ILoggingBroker, returningFunction: () => Promise<T>): Promise<T> {
        try {
            return await returningFunction();
        } catch (error: unknown) {
            if (error instanceof Xeption) {
                const wrapper = new IdentityViewValidationException(
                    "Identity view validation error occurred, please check your input.", 
                    error
                );
                await loggingBroker.logErrorAsync(wrapper);
                throw wrapper;
            }
            throw (error instanceof Error) ? error : new Error(String(error));
        }
    }
}
