// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { apiBroker, loggingBroker } from '../../app/core';
import { PaymentMethodService } from '../../src/services/foundations/paymentMethods/PaymentMethodService';
import { WalletViewService } from '../../src/services/views/walletViews/WalletViewService';

const paymentMethodService = new PaymentMethodService(apiBroker, loggingBroker);
const walletViewService = new WalletViewService(paymentMethodService, loggingBroker);

export { walletViewService };
