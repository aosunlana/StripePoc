// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class ServiceSelectionView {
    public serviceName: string;
    public amount: number;
    public currency: string;

    constructor(init?: Partial<ServiceSelectionView>) {
        if (init) Object.assign(this, init);
    }
}
