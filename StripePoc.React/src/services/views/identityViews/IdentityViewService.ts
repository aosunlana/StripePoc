// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IAccountService } from '../../foundations/accounts/IAccountService';
import type { IIdentityViewService } from './IIdentityViewService';
import { BusinessView } from '../../../models/views/BusinessView';
import { AccountView } from '../../../models/views/AccountView';
import { IdentityViewServiceExceptions } from './IdentityViewService.Exceptions';

export class IdentityViewService extends IdentityViewServiceExceptions implements IIdentityViewService {
    constructor(
        private readonly accountService: IAccountService,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async retrieveAllBusinessesViewAsync(): Promise<BusinessView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.accountService.retrieveAllBusinessesAsync();
        });
    }

    public async retrieveAccountsByBusinessIdViewAsync(businessId: string): Promise<AccountView[]> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.accountService.retrieveAccountsByBusinessIdAsync(businessId);
        });
    }

    public async verifyIdentityViewAsync(accountId: string): Promise<{ stripeCustomerId: string }> {
        return await this.tryCatch(this.loggingBroker, async () => {
             return await this.accountService.providePaymentAccountAsync(accountId);
        });
    }
}
