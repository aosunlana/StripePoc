// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { PaymentLifecycleEventView } from '../../../models/views/PaymentLifecycleEventView';
import type { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';

export interface IActionViewService {
    directChargeViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
    requestSubscriptionQuoteViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
    requestOneTimeQuoteViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView>;
}
