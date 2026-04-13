// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../models/xeptions/Xeption';
import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import { AccountValidationException } from '../../../models/foundations/accounts/exceptions/AccountValidationException';
import { AccountDependencyException } from '../../../models/foundations/accounts/exceptions/AccountDependencyException';
import { AccountServiceException } from '../../../models/foundations/accounts/exceptions/AccountServiceException';

export class AccountServiceExceptions {
    protected async tryCatch<T>(loggingBroker: ILoggingBroker, returningFunction: () => Promise<T>): Promise<T> {
        try {
            return await returningFunction();
        } catch (error: unknown) {
            if (error instanceof Xeption) {
                const accountValidationException = new AccountValidationException(
                    "Account validation error occurred, fix errors and try again.", 
                    error
                );
                await loggingBroker.logErrorAsync(accountValidationException);
                throw accountValidationException;
            }

            const axiosError = error as { response?: unknown };
            if (axiosError?.response) {
                 const accountDependencyException = new AccountDependencyException(
                    "Account dependency error occurred, contact support.", 
                    error as Error
                 );
                 await loggingBroker.logErrorAsync(accountDependencyException);
                 throw accountDependencyException;
            }

            const accountServiceException = new AccountServiceException(
                "Account service error occurred, contact support.", 
                (error instanceof Error) ? error : new Error(String(error))
            );
            await loggingBroker.logErrorAsync(accountServiceException);
            throw accountServiceException;
        }
    }
}
