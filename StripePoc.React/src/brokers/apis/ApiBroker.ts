// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import axios, { type AxiosInstance } from 'axios';
import type { IApiBroker } from './IApiBroker';
import type { PaymentLifecycleEventView } from '../../models/views/PaymentLifecycleEventView';
import type { PaymentMethodView } from '../../models/views/PaymentMethodView';
import type { AccountView } from '../../models/views/AccountView';
import type { BusinessView } from '../../models/views/BusinessView';
import type { SubscriptionPayload } from '../../models/foundations/payments/SubscriptionPayload';
import type { OneTimePaymentPayload } from '../../models/foundations/payments/OneTimePaymentPayload';
import type { PaymentView } from '../../models/foundations/payments/PaymentView';

export class ApiBroker implements IApiBroker {
    private apiClient: AxiosInstance;

    constructor() {
        this.apiClient = axios.create({
            baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000',
            headers: {
                'Content-Type': 'application/json',
            },
        });
    }

    public async getBusinessesAsync(): Promise<BusinessView[]> {
        return await this.get<BusinessView[]>('/api/Account/businesses');
    }

    public async getAccountByIdAsync(accountId: string): Promise<AccountView> {
        const accounts = await this.get<AccountView[]>('/api/Account/all');
        const account = accounts.find(a => a.id === accountId);
        
        if (!account) {
            throw new Error(`Account with ID ${accountId} not found.`);
        }

        return account;
    }

    public async getAccountsByBusinessAsync(businessId: string): Promise<AccountView[]> {
        return await this.get<AccountView[]>(`/api/Account/business/${businessId}`);
    }

    public async providePaymentAccountAsync(accountId: string): Promise<{ stripeCustomerId: string }> {
        return await this.post<{ stripeCustomerId: string }>(`/api/PaymentAccounts/${accountId}`);
    }

    public async getPaymentMethodsAsync(accountId: string): Promise<PaymentMethodView[]> {
        return await this.get<PaymentMethodView[]>(`/api/PaymentMethods/accounts/${accountId}`);
    }

    public async initiateCardSetupAsync(accountId: string): Promise<{ clientSecret: string }> {
        // Backend returns the string directly, not an object
        const clientSecret = await this.post<string>(`/api/PaymentMethods/accounts/${accountId}/setup/initiate`, {});
        return { clientSecret };
    }

    public async finaliseCardSetupAsync(accountId: string, paymentMethodId: string): Promise<PaymentMethodView> {
        // Backend expects the PaymentMethod object with StripePaymentMethodId (PascalCase)
        return await this.post<PaymentMethodView>(`/api/PaymentMethods/accounts/${accountId}`, {
            StripePaymentMethodId: paymentMethodId
        });
    }

    public async createSubscriptionAsync(payload: SubscriptionPayload): Promise<PaymentLifecycleEventView> {
        return await this.post<PaymentLifecycleEventView>('/api/Subscriptions', {
            AccountId: payload.accountId,
            ServiceName: payload.serviceName,
            Amount: payload.amount,
            Currency: payload.currency,
            StripePriceId: payload.stripePriceId
        });
    }

    public async createSubscriptionQuoteAsync(payload: SubscriptionPayload): Promise<PaymentLifecycleEventView> {
        return await this.post<PaymentLifecycleEventView>('/api/Subscriptions/quote', {
            AccountId: payload.accountId,
            ServiceName: payload.serviceName,
            Amount: payload.amount,
            Currency: payload.currency,
            StripePriceId: payload.stripePriceId
        });
    }

    public async processOneTimePaymentAsync(payload: OneTimePaymentPayload): Promise<PaymentLifecycleEventView> {
        return await this.post<PaymentLifecycleEventView>('/api/one-time-payments', {
            AccountId: payload.accountId,
            Amount: payload.amount,
            Currency: payload.currency,
            Status: 0 // Defaulting to the expected C# Enum value for processing
        });
    }

    public async processOneTimeQuotePaymentAsync(payload: OneTimePaymentPayload): Promise<PaymentLifecycleEventView> {
        return await this.post<PaymentLifecycleEventView>('/api/one-time-payments/quote', {
            AccountId: payload.accountId,
            Amount: payload.amount,
            Currency: payload.currency,
            Status: 0
        });
    }

    public async getPaymentHistoryAsync(accountId: string): Promise<PaymentView[]> {
        return await this.get<PaymentView[]>(`/api/Payments/accounts/${accountId}`);
    }

    private async get<T>(url: string): Promise<T> {
        const response = await this.apiClient.get<T>(url);
        return response.data;
    }

    private async post<T>(url: string, data?: Record<string, unknown> | object): Promise<T> {
        const response = await this.apiClient.post<T>(url, data);
        return response.data;
    }
}
