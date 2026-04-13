// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { ApiBroker } from '../src/brokers/apis/ApiBroker';
import { LoggingBroker } from '../src/brokers/loggings/LoggingBroker';
import { DateTimeBroker } from '../src/brokers/dateTimes/DateTimeBroker';

const apiBroker = new ApiBroker();
const loggingBroker = new LoggingBroker();
const dateTimeBroker = new DateTimeBroker();

export { apiBroker, loggingBroker, dateTimeBroker };
