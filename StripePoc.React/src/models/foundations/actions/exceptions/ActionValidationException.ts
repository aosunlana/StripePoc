// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class ActionValidationException extends Xeption {
    constructor(message: string, innerException: Xeption) {
        super(message, innerException);
    }
}
