// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class AccountServiceException extends Xeption {
    constructor(message: string, innerException: Error) {
        super(message, innerException);
    }
}
