// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../models/xeptions/Xeption';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import { ActionValidationException } from '../../../models/foundations/actions/exceptions/ActionValidationException';

export class ActionViewServiceExceptions {
    protected async tryCatch<T>(loggingBroker: ILoggingBroker, returningFunction: () => Promise<T>): Promise<T> {
        try {
            return await returningFunction();
        } catch (error: unknown) {
            if (error instanceof Xeption) {
                const wrapper = new ActionValidationException("Action validation error occurred.", error);
                await loggingBroker.logErrorAsync(wrapper);
                throw wrapper;
            }
            throw (error instanceof Error) ? error : new Error(String(error));
        }
    }
}
