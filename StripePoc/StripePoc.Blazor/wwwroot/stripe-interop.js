window.stripeInterop = {
    initialize: function (publishableKey) {
        this.stripe = Stripe(publishableKey);
        this.elements = this.stripe.elements();
    },
    createCardElement: function (elementId) {
        this.card = this.elements.create('card', {
            style: {
                base: {
                    color: '#32325d',
                    fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
                    fontSmoothing: 'antialiased',
                    fontSize: '16px',
                    '::placeholder': {
                        color: '#aab7c4'
                    }
                },
                invalid: {
                    color: '#fa755a',
                    iconColor: '#fa755a'
                }
            }
        });
        this.card.mount('#' + elementId);
    },
    createPaymentMethod: async function () {
        const { paymentMethod, error } = await this.stripe.createPaymentMethod({
            type: 'card',
            card: this.card,
        });

        if (error) {
            console.error(error);
            return { error: error.message };
        } else {
            return { id: paymentMethod.id };
        }
    },
    confirmCardSetup: async function (clientSecret, billingDetails) {
        const { setupIntent, error } = await this.stripe.confirmCardSetup(
            clientSecret,
            {
                payment_method: {
                    card: this.card,
                    billing_details: billingDetails
                }
            }
        );

        if (error) {
            console.error(error);
            return { error: error.message };
        } else {
            // payment_method can be a string ID or an expanded object
            const pmId = typeof setupIntent.payment_method === 'string'
                ? setupIntent.payment_method
                : setupIntent.payment_method.id;
            console.log('Card setup succeeded. PaymentMethod ID:', pmId);
            return { id: pmId };
        }
    },
    confirmCardPayment: async function (clientSecret) {
        const { paymentIntent, error } = await this.stripe.confirmCardPayment(clientSecret);

        if (error) {
            console.error(error);
            return { error: error.message };
        } else {
            return { id: paymentIntent.id, status: paymentIntent.status };
        }
    }
};
