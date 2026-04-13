// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export interface IDateTimeBroker {
    getCurrentDateTimeOffsetAsync(): Promise<Date>;
}
