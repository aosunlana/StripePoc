// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export class InvalidPaymentException extends Error {
    innerException: Error | null;
    constructor(message: string, innerException: Error | null = null) {
        super(message);
        this.name = 'InvalidPaymentException';
        this.innerException = innerException;
    }
}

export class PaymentValidationException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'PaymentValidationException';
        this.innerException = innerException;
    }
}

export class FailedPaymentDependencyException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'FailedPaymentDependencyException';
        this.innerException = innerException;
    }
}

export class PaymentDependencyException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'PaymentDependencyException';
        this.innerException = innerException;
    }
}

export class FailedPaymentServiceException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'FailedPaymentServiceException';
        this.innerException = innerException;
    }
}

export class PaymentServiceException extends Error {
    innerException: Error;
    constructor(message: string, innerException: Error) {
        super(message);
        this.name = 'PaymentServiceException';
        this.innerException = innerException;
    }
}
