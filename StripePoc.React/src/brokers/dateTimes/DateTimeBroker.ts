// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { IDateTimeBroker } from './IDateTimeBroker';

export class DateTimeBroker implements IDateTimeBroker {
    public async getCurrentDateTimeOffsetAsync(): Promise<Date> {
        return new Date();
    }
}
