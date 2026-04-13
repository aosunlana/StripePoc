// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../models/xeptions/Xeption';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';

export class WalletViewServiceExceptions {
    protected async tryCatch<T>(loggingBroker: ILoggingBroker, returningFunction: () => Promise<T>): Promise<T> {
        try {
            return await returningFunction();
        } catch (error: unknown) {
             if (error instanceof Xeption) {
                // Map to domain specific if needed, for now rethrow
                await loggingBroker.logErrorAsync(error);
                throw error;
            }
            throw (error instanceof Error) ? error : new Error(String(error));
        }
    }
}
