// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { ILoggingBroker } from './ILoggingBroker';

export class LoggingBroker implements ILoggingBroker {
    public async logInformationAsync(message: string): Promise<void> {
        console.info(message);
    }

    public async logTraceAsync(message: string): Promise<void> {
        console.trace(message);
    }

    public async logDebugAsync(message: string): Promise<void> {
        console.debug(message);
    }

    public async logWarningAsync(message: string): Promise<void> {
        console.warn(message);
    }

    public async logErrorAsync(exception: Error): Promise<void> {
        console.error(exception.message, exception);
    }

    public async logCriticalAsync(exception: Error): Promise<void> {
        console.error(`CRITICAL: ${exception.message}`, exception);
    }
}
