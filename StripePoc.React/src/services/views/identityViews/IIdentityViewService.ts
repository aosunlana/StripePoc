// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { BusinessView } from '../../../models/views/BusinessView';
import type { AccountView } from '../../../models/views/AccountView';

export interface IIdentityViewService {
    retrieveAllBusinessesViewAsync(): Promise<BusinessView[]>;
    retrieveAccountsByBusinessIdViewAsync(businessId: string): Promise<AccountView[]>;
    verifyIdentityViewAsync(accountId: string): Promise<{ stripeCustomerId: string }>;
}
