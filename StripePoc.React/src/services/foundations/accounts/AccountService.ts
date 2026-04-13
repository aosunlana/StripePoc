// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IApiBroker } from '../../../brokers/apis/IApiBroker';
import type { IAccountService } from './IAccountService';
import { BusinessView } from '../../../models/views/BusinessView';
import { AccountView } from '../../../models/views/AccountView';
import { AccountServiceExceptions } from './AccountService.Exceptions';

export class AccountService extends AccountServiceExceptions implements IAccountService {
    constructor(
        private readonly apiBroker: IApiBroker,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async retrieveAllBusinessesAsync(): Promise<BusinessView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.apiBroker.getBusinessesAsync();
        });
    }

    public async retrieveAccountsByBusinessIdAsync(businessId: string): Promise<AccountView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateBusinessId(businessId);
            return await this.apiBroker.getAccountsByBusinessAsync(businessId);
        });
    }

    public async providePaymentAccountAsync(accountId: string): Promise<{ stripeCustomerId: string }> {
        return await this.tryCatch(this.loggingBroker, async () => {
            this.validateAccountId(accountId);
            return await this.apiBroker.providePaymentAccountAsync(accountId);
        });
    }

    /* Note: Validations are imported/called from AccountService.Validations (via inheritance or helper) */
    /* Implementation detail: To keep it clean in TS, I'll put the validation logic as protected methods 
       in a base class to simulate the partial class behavior. */
    
    private validateBusinessId(businessId: string): void {
        if (!businessId) {
            // Error gathering would go here in a more complex validation
            throw new Error("Business ID is required");
        }
    }

    private validateAccountId(accountId: string): void {
        if (!accountId) {
            throw new Error("Account ID is required");
        }
    }
}
