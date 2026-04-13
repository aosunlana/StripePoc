// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { apiBroker, loggingBroker } from '../../app/core';
import { AccountService } from '../../src/services/foundations/accounts/AccountService';
import { IdentityViewService } from '../../src/services/views/identityViews/IdentityViewService';

const accountService = new AccountService(apiBroker, loggingBroker);
const identityViewService = new IdentityViewService(accountService, loggingBroker);

export { identityViewService };
