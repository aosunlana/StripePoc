// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class BusinessView {
    public id: string;
    public name: string;

    constructor(init?: Partial<BusinessView>) {
        if (init) Object.assign(this, init);
    }
}
