// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class InvalidAccountException extends Xeption {
    constructor(message: string) {
        super(message);
    }
}
