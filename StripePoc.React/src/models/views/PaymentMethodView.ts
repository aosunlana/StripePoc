// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class PaymentMethodView {
    public id: string;
    public type: string;
    public brand: string;
    public last4: string;
    public expiryMonth: number;
    public expiryYear: number;
    public isDefault: boolean;

    constructor(init?: Partial<PaymentMethodView>) {
        if (init) Object.assign(this, init);
    }
}
