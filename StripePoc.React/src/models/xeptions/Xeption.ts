// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class Xeption extends Error {
    public data: Map<string, string[]> = new Map<string, string[]>();

    constructor(message: string = "", innerException?: Error) {
        super(message);
        this.name = this.constructor.name;
        
        // Restore prototype chain for instanceof
        Object.setPrototypeOf(this, new.target.prototype);

        if (innerException) {
            this.stack = `${this.stack}\nInner Exception: ${innerException.stack}`;
        }
    }

    public upsertDataList(key: string, value: string): void {
        const existingValues = this.data.get(key) || [];
        existingValues.push(value);
        this.data.set(key, existingValues);
    }

    public throwIfContainsErrors(): void {
        if (this.data.size > 0) {
            throw this;
        }
    }

    public addData(key: string, ...values: string[]): void {
        this.data.set(key, values);
    }

    public hasErrors(): boolean {
        return this.data.size > 0;
    }
}
