// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import type { PaymentLifecycleEventView } from '../../models/views/PaymentLifecycleEventView';
import type { PaymentMethodView } from '../../models/views/PaymentMethodView';
import type { AccountView } from '../../models/views/AccountView';
import type { BusinessView } from '../../models/views/BusinessView';
import type { SubscriptionPayload } from '../../models/foundations/payments/SubscriptionPayload';
import type { OneTimePaymentPayload } from '../../models/foundations/payments/OneTimePaymentPayload';
import type { PaymentView } from '../../models/foundations/payments/PaymentView';

export interface IApiBroker {
    // Businesses
    getBusinessesAsync(): Promise<BusinessView[]>;

    // Accounts
    getAccountByIdAsync(accountId: string): Promise<AccountView>;
    getAccountsByBusinessAsync(businessId: string): Promise<AccountView[]>;
    providePaymentAccountAsync(accountId: string): Promise<{ stripeCustomerId: string }>;
    
    // Payment Methods
    getPaymentMethodsAsync(accountId: string): Promise<PaymentMethodView[]>;
    initiateCardSetupAsync(accountId: string): Promise<{ clientSecret: string }>;
    finaliseCardSetupAsync(accountId: string, paymentMethodId: string): Promise<PaymentMethodView>;

    // Subscriptions
    createSubscriptionAsync(payload: SubscriptionPayload): Promise<PaymentLifecycleEventView>;
    createSubscriptionQuoteAsync(payload: SubscriptionPayload): Promise<PaymentLifecycleEventView>;

    // One-time Payments
    processOneTimePaymentAsync(payload: OneTimePaymentPayload): Promise<PaymentLifecycleEventView>;
    processOneTimeQuotePaymentAsync(payload: OneTimePaymentPayload): Promise<PaymentLifecycleEventView>;

    // Payments History
    getPaymentHistoryAsync(accountId: string): Promise<PaymentView[]>;
}
