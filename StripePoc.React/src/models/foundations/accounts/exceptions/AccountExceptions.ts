// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class AccountValidationException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'AccountValidationException';
        this.innerException = innerException;
    }
}

export class FailedAccountDependencyException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'FailedAccountDependencyException';
        this.innerException = innerException;
    }
}

export class AccountDependencyException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'AccountDependencyException';
        this.innerException = innerException;
    }
}

export class FailedAccountServiceException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'FailedAccountServiceException';
        this.innerException = innerException;
    }
}

export class AccountServiceException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'AccountServiceException';
        this.innerException = innerException;
    }
}
