// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { apiBroker, loggingBroker } from '../../app/core';
import { ActionService } from '../../src/services/foundations/actions/ActionService';
import { ActionViewService } from '../../src/services/views/actionViews/ActionViewService';

const actionService = new ActionService(apiBroker, loggingBroker);
const actionViewService = new ActionViewService(actionService, loggingBroker);

export { actionViewService };
