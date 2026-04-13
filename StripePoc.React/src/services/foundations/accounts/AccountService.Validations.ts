// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { InvalidAccountException } from '../../../models/foundations/accounts/exceptions/InvalidAccountException';

export class AccountServiceValidations {
    protected validateBusinessId(businessId: string): void {
        const invalidAccountException = new InvalidAccountException("Invalid account, fix errors and try again.");

        if (!businessId) {
            invalidAccountException.upsertDataList("businessId", "Business ID is required");
        }

        invalidAccountException.throwIfContainsErrors();
    }

    protected validateAccountId(accountId: string): void {
        const invalidAccountException = new InvalidAccountException("Invalid account, fix errors and try again.");

        if (!accountId) {
            invalidAccountException.upsertDataList("accountId", "Account ID is required");
        }

        invalidAccountException.throwIfContainsErrors();
    }
}
