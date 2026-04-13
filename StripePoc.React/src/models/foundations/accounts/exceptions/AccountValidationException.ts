// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class AccountValidationException extends Xeption {
    constructor(message: string, innerException: Xeption) {
        super(message, innerException);
    }
}
