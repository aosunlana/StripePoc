// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { loggingBroker } from '../../app/core';
import { PricingViewService } from '../../src/services/views/pricingViews/PricingViewService';

const pricingViewService = new PricingViewService(loggingBroker);

export { pricingViewService };
