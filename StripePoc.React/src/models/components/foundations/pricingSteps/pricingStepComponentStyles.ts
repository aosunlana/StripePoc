// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const PricingStepComponentStyles = {
    container: { selector: '.pricing-step-container', display: 'grid', gap: '1rem' },
    header: { selector: '.pricing-step-header', display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '2rem' },
    iconWrap: { selector: '.pricing-step-icon-wrap', padding: '0.75rem', background: 'var(--muted)', borderRadius: 'var(--radius)', display: 'flex' },
    title: { selector: '.pricing-step-title', fontSize: '1.5rem', fontWeight: '700', margin: '0' },
    subtitle: { selector: '.pricing-step-subtitle', color: 'var(--secondary)', margin: '0' },
    plansGrid: { selector: '.pricing-step-plans', display: 'grid', gap: '1rem', marginBottom: '2.5rem' },
    summary: { selector: '.pricing-step-summary', background: 'var(--muted)', padding: '1.5rem', borderRadius: 'var(--radius)', marginBottom: '2.5rem' },
    summaryTitle: { selector: '.pricing-step-summary-title', display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '1rem', color: 'var(--primary)', fontWeight: '600' },
    summaryRow: { selector: '.pricing-step-summary-row', display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem', marginBottom: '0.5rem' },
    summaryTotal: { selector: '.pricing-step-summary-total', display: 'flex', justifyContent: 'space-between', fontSize: '1.125rem', fontWeight: '800', marginTop: '0.5rem', paddingTop: '0.5rem', borderTop: '1px solid var(--border)' },
    footer: { selector: '.pricing-step-footer', display: 'flex', justifyContent: 'space-between' },
};
