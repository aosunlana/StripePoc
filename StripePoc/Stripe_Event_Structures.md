# Stripe Event Structure Analysis

Generated on: 2026-04-11 04:25:27

This document lists the properties of common Stripe objects used in webhook events, filtered for identity, financial data, and statuses.

## Invoice

| Property | Type | Details |
| :--- | :--- | :--- |
| AccountCountry | String | Value |
| AccountName | String | Value |
| AccountTaxIdIds | List`1 | Object |
| AccountTaxIds | List`1 | Object |
| AmountDue | Int64 | Value |
| AmountOverpaid | Int64 | Value |
| AmountPaid | Int64 | Value |
| AmountRemaining | Int64 | Value |
| AmountShipping | Int64 | Value |
| ApplicationId | String | Value |
| AttemptCount | Int64 | Value |
| CollectionMethod | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerAddress | Address | Object |
| CustomerEmail | String | Value |
| CustomerId | String | Value |
| CustomerName | String | Value |
| CustomerPhone | String | Value |
| CustomerShipping | Shipping | Object |
| CustomerTaxExempt | String | Value |
| CustomerTaxIds | List`1 | Object |
| DefaultPaymentMethod | PaymentMethod | Object |
| DefaultPaymentMethodId | String | Value |
| DefaultSourceId | String | Value |
| DiscountIds | List`1 | Object |
| Discounts | List`1 | Object |
| DueDate | DateTime? | Object |
| FromInvoice | InvoiceFromInvoice | Object |
| HostedInvoiceUrl | String | Value |
| Id | String | Value |
| InvoicePdf | String | Value |
| LatestRevisionId | String | Value |
| Metadata | Dictionary`2 | Object |
| NextPaymentAttempt | DateTime? | Object |
| OnBehalfOfId | String | Value |
| Payments | StripeList`1 | Object |
| PaymentSettings | InvoicePaymentSettings | Object |
| PeriodEnd | DateTime | Value |
| PeriodStart | DateTime | Value |
| PostPaymentCreditNotesAmount | Int64 | Value |
| PrePaymentCreditNotesAmount | Int64 | Value |
| Status | String | Value |
| StatusTransitions | InvoiceStatusTransitions | Object |
| Subtotal | Int64 | Value |
| SubtotalExcludingTax | Int64? | Value |
| TestClockId | String | Value |
| Total | Int64 | Value |
| TotalDiscountAmounts | List`1 | Object |
| TotalExcludingTax | Int64? | Value |
| TotalPretaxCreditAmounts | List`1 | Object |
| TotalTaxes | List`1 | Object |

## PaymentIntent

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| AmountCapturable | Int64 | Value |
| AmountDetails | PaymentIntentAmountDetails | Object |
| AmountReceived | Int64 | Value |
| ApplicationFeeAmount | Int64? | Value |
| ApplicationId | String | Value |
| AutomaticPaymentMethods | PaymentIntentAutomaticPaymentMethods | Object |
| CaptureMethod | String | Value |
| ConfirmationMethod | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerId | String | Value |
| ExcludedPaymentMethodTypes | List`1 | Object |
| Id | String | Value |
| LastPaymentError | StripeError | Object |
| LatestChargeId | String | Value |
| Metadata | Dictionary`2 | Object |
| OnBehalfOfId | String | Value |
| PaymentDetails | PaymentIntentPaymentDetails | Object |
| PaymentMethod | PaymentMethod | Object |
| PaymentMethodConfigurationDetails | PaymentIntentPaymentMethodConfigurationDetails | Object |
| PaymentMethodId | String | Value |
| PaymentMethodOptions | PaymentIntentPaymentMethodOptions | Object |
| PaymentMethodTypes | List`1 | Object |
| ReviewId | String | Value |
| SourceId | String | Value |
| Status | String | Value |

## Subscription

| Property | Type | Details |
| :--- | :--- | :--- |
| ApplicationId | String | Value |
| CancelAtPeriodEnd | Boolean | Value |
| CollectionMethod | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerId | String | Value |
| DefaultPaymentMethod | PaymentMethod | Object |
| DefaultPaymentMethodId | String | Value |
| DefaultSourceId | String | Value |
| DiscountIds | List`1 | Object |
| Discounts | List`1 | Object |
| Id | String | Value |
| InvoiceSettings | SubscriptionInvoiceSettings | Object |
| LatestInvoice | Invoice | Object |
| LatestInvoiceId | String | Value |
| Metadata | Dictionary`2 | Object |
| NextPendingInvoiceItemInvoice | DateTime? | Object |
| OnBehalfOfId | String | Value |
| PaymentSettings | SubscriptionPaymentSettings | Object |
| PendingInvoiceItemInterval | SubscriptionPendingInvoiceItemInterval | Object |
| PendingSetupIntentId | String | Value |
| PendingUpdate | SubscriptionPendingUpdate | Object |
| ScheduleId | String | Value |
| StartDate | DateTime | Value |
| Status | String | Value |
| TestClockId | String | Value |

## Charge

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| AmountCaptured | Int64 | Value |
| AmountRefunded | Int64 | Value |
| ApplicationFeeAmount | Int64? | Value |
| ApplicationFeeId | String | Value |
| ApplicationId | String | Value |
| BalanceTransactionId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| Customer | Customer | Object |
| CustomerId | String | Value |
| FailureBalanceTransactionId | String | Value |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |
| OnBehalfOfId | String | Value |
| Paid | Boolean | Value |
| PaymentIntent | PaymentIntent | Object |
| PaymentIntentId | String | Value |
| PaymentMethod | String | Value |
| PaymentMethodDetails | ChargePaymentMethodDetails | Object |
| Refunded | Boolean | Value |
| Refunds | StripeList`1 | Object |
| ReviewId | String | Value |
| SourceTransferId | String | Value |
| Status | String | Value |
| TransferId | String | Value |

## Refund

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| BalanceTransactionId | String | Value |
| ChargeId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| FailureBalanceTransactionId | String | Value |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |
| PaymentIntent | PaymentIntent | Object |
| PaymentIntentId | String | Value |
| SourceTransferReversalId | String | Value |
| Status | String | Value |
| TransferReversalId | String | Value |

## Customer

| Property | Type | Details |
| :--- | :--- | :--- |
| Created | DateTime | Value |
| Currency | String | Value |
| CustomerAccount | String | Value |
| DefaultSourceId | String | Value |
| Discount | Discount | Object |
| Id | String | Value |
| IndividualName | String | Value |
| InvoiceCreditBalance | Dictionary`2 | Object |
| InvoicePrefix | String | Value |
| InvoiceSettings | CustomerInvoiceSettings | Object |
| Metadata | Dictionary`2 | Object |
| NextInvoiceSequence | Int64 | Value |
| Subscriptions | StripeList`1 | Object |
| TaxIds | StripeList`1 | Object |
| TestClockId | String | Value |

## PaymentMethod

| Property | Type | Details |
| :--- | :--- | :--- |
| Created | DateTime | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerBalance | PaymentMethodCustomerBalance | Object |
| CustomerId | String | Value |
| Id | String | Value |
| Ideal | PaymentMethodIdeal | Object |
| Metadata | Dictionary`2 | Object |
| NzBankAccount | PaymentMethodNzBankAccount | Object |
| Type | String | Value |
| UsBankAccount | PaymentMethodUsBankAccount | Object |

## SetupIntent

| Property | Type | Details |
| :--- | :--- | :--- |
| ApplicationId | String | Value |
| AutomaticPaymentMethods | SetupIntentAutomaticPaymentMethods | Object |
| Created | DateTime | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerId | String | Value |
| ExcludedPaymentMethodTypes | List`1 | Object |
| Id | String | Value |
| LatestAttemptId | String | Value |
| Mandate | Mandate | Object |
| MandateId | String | Value |
| Metadata | Dictionary`2 | Object |
| OnBehalfOfId | String | Value |
| PaymentMethod | PaymentMethod | Object |
| PaymentMethodConfigurationDetails | SetupIntentPaymentMethodConfigurationDetails | Object |
| PaymentMethodId | String | Value |
| PaymentMethodOptions | SetupIntentPaymentMethodOptions | Object |
| PaymentMethodTypes | List`1 | Object |
| SingleUseMandate | Mandate | Object |
| SingleUseMandateId | String | Value |
| Status | String | Value |

## Payout

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| ApplicationFeeAmount | Int64? | Value |
| ApplicationFeeId | String | Value |
| ArrivalDate | DateTime | Value |
| BalanceTransactionId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| DestinationId | String | Value |
| FailureBalanceTransactionId | String | Value |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |
| Method | String | Value |
| OriginalPayoutId | String | Value |
| PayoutMethod | String | Value |
| ReconciliationStatus | String | Value |
| ReversedById | String | Value |
| SourceType | String | Value |
| Status | String | Value |
| TraceId | PayoutTraceId | Object |
| Type | String | Value |

## Transfer

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| AmountReversed | Int64 | Value |
| BalanceTransactionId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| DestinationId | String | Value |
| DestinationPayment | Charge | Object |
| DestinationPaymentId | String | Value |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |
| SourceTransactionId | String | Value |
| SourceType | String | Value |

## Balance

| Property | Type | Details |
| :--- | :--- | :--- |
| RefundAndDisputePrefunding | BalanceRefundAndDisputePrefunding | Object |

## Dispute

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64 | Value |
| ChargeId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| EnhancedEligibilityTypes | List`1 | Object |
| Evidence | DisputeEvidence | Object |
| EvidenceDetails | DisputeEvidenceDetails | Object |
| Id | String | Value |
| IsChargeRefundable | Boolean | Value |
| Metadata | Dictionary`2 | Object |
| PaymentIntent | PaymentIntent | Object |
| PaymentIntentId | String | Value |
| PaymentMethodDetails | DisputePaymentMethodDetails | Object |
| Status | String | Value |

## Session

| Property | Type | Details |
| :--- | :--- | :--- |
| AmountSubtotal | Int64? | Value |
| AmountTotal | Int64? | Value |
| ClientReferenceId | String | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| CurrencyConversion | SessionCurrencyConversion | Object |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerCreation | String | Value |
| CustomerDetails | SessionCustomerDetails | Object |
| CustomerEmail | String | Value |
| CustomerId | String | Value |
| Discounts | List`1 | Object |
| ExcludedPaymentMethodTypes | List`1 | Object |
| Id | String | Value |
| IntegrationIdentifier | String | Value |
| Invoice | Invoice | Object |
| InvoiceCreation | SessionInvoiceCreation | Object |
| InvoiceId | String | Value |
| Metadata | Dictionary`2 | Object |
| PaymentIntent | PaymentIntent | Object |
| PaymentIntentId | String | Value |
| PaymentLink | PaymentLink | Object |
| PaymentLinkId | String | Value |
| PaymentMethodCollection | String | Value |
| PaymentMethodConfigurationDetails | SessionPaymentMethodConfigurationDetails | Object |
| PaymentMethodOptions | SessionPaymentMethodOptions | Object |
| PaymentMethodTypes | List`1 | Object |
| PaymentStatus | String | Value |
| SavedPaymentMethodOptions | SessionSavedPaymentMethodOptions | Object |
| SetupIntentId | String | Value |
| Status | String | Value |
| SubmitType | String | Value |
| Subscription | Subscription | Object |
| SubscriptionId | String | Value |
| SuccessUrl | String | Value |
| TaxIdCollection | SessionTaxIdCollection | Object |
| TotalDetails | SessionTotalDetails | Object |

## PromotionCode

| Property | Type | Details |
| :--- | :--- | :--- |
| Created | DateTime | Value |
| Customer | Customer | Object |
| CustomerAccount | String | Value |
| CustomerId | String | Value |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |

## Coupon

| Property | Type | Details |
| :--- | :--- | :--- |
| AmountOff | Int64? | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| CurrencyOptions | Dictionary`2 | Object |
| Id | String | Value |
| Metadata | Dictionary`2 | Object |
| Valid | Boolean | Value |

## Source

| Property | Type | Details |
| :--- | :--- | :--- |
| Amount | Int64? | Value |
| Created | DateTime | Value |
| Currency | String | Value |
| Customer | String | Value |
| Id | String | Value |
| Ideal | SourceIdeal | Object |
| Metadata | Dictionary`2 | Object |
| Status | String | Value |
| Type | String | Value |
