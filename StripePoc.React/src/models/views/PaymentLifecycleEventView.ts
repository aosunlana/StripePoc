// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class PaymentLifecycleEventView {
    public isValid: boolean;
    public eventType: string;
    public status: string;
    public externalReferenceId: string;
    public amount: number;
    public currency: string;
    public checkoutUrl: string;
    public message?: string;
    public occurredAt: string;

    constructor(init?: Partial<PaymentLifecycleEventView>) {
        if (init) Object.assign(this, init);
    }
}
