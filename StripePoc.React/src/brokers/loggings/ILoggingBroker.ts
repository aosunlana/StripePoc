// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export interface ILoggingBroker {
    logInformationAsync(message: string): Promise<void>;
    logTraceAsync(message: string): Promise<void>;
    logDebugAsync(message: string): Promise<void>;
    logWarningAsync(message: string): Promise<void>;
    logErrorAsync(exception: Error): Promise<void>;
    logCriticalAsync(exception: Error): Promise<void>;
}
