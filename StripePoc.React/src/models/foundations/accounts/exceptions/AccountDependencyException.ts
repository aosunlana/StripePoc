// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class AccountDependencyException extends Xeption {
    constructor(message: string, innerException: Error) {
        super(message, innerException);
    }
}
