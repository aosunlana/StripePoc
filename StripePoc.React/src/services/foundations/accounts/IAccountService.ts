// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { BusinessView } from '../../../models/views/BusinessView';
import type { AccountView } from '../../../models/views/AccountView';

export interface IAccountService {
    retrieveAllBusinessesAsync(): Promise<BusinessView[]>;
    retrieveAccountsByBusinessIdAsync(businessId: string): Promise<AccountView[]>;
    providePaymentAccountAsync(accountId: string): Promise<{ stripeCustomerId: string }>;
}
