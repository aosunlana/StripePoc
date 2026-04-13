// ---------------------------------------------------------------
// Copyright (c) Dream Labs Innovations. All rights reserved.
// ---------------------------------------------------------------

export class ExceptionBase extends Error {
    constructor(message, innerException = null) {
        super(message);
        this.name = this.constructor.name;
        this.innerException = innerException;
        this.data = {};
    }

    addData(key, value) {
        if (!this.data[key]) {
            this.data[key] = [];
        }
        this.data[key].push(value);
    }
}
