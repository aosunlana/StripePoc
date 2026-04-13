// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from '../../../brokers/loggings/ILoggingBroker';
import type { IActionService } from '../../foundations/actions/IActionService';
import type { IActionViewService } from './IActionViewService';
import { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';
import { PaymentLifecycleEventView } from '../../../models/views/PaymentLifecycleEventView';
import { ActionViewServiceExceptions } from './ActionViewService.Exceptions';

export class ActionViewService extends ActionViewServiceExceptions implements IActionViewService {
    constructor(
        private readonly actionService: IActionService,
        private readonly loggingBroker: ILoggingBroker
    ) {
        super();
    }

    public async directChargeViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.actionService.directChargeAsync(accountId, selection);
        });
    }

    public async requestSubscriptionQuoteViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.actionService.requestSubscriptionQuoteAsync(accountId, selection);
        });
    }

    public async requestOneTimeQuoteViewAsync(accountId: string, selection: ServiceSelectionView): Promise<PaymentLifecycleEventView> {
        return await this.tryCatch(this.loggingBroker, async () => {
            return await this.actionService.requestOneTimeQuoteAsync(accountId, selection);
        });
    }
}
