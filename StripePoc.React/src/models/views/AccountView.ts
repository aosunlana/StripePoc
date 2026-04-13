// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class AccountView {
    public id: string;
    public name: string;

    constructor(init?: Partial<AccountView>) {
        if (init) Object.assign(this, init);
    }
}
