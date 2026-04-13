// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class IdentityViewValidationException extends Xeption {
    constructor(message: string, innerException: Xeption) {
        super(message, innerException);
    }
}
