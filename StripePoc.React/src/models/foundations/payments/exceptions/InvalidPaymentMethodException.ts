// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { Xeption } from '../../../xeptions/Xeption';

export class InvalidPaymentMethodException extends Xeption {
    constructor(message: string) {
        super(message);
    }
}
