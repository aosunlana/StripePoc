// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { PaymentLifecycleEventView } from '../../../models/views/PaymentLifecycleEventView';
import type { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';

export interface IActionService {
    directChargeAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
    requestSubscriptionQuoteAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
    requestOneTimeQuoteAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
}
