// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const ActionStepComponentStyles = {
    container: { selector: '.action-step-container', display: 'grid', gap: '1.5rem' },
    header: { selector: '.action-step-header', display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '2.5rem' },
    iconWrap: { selector: '.action-step-icon-wrap', padding: '0.75rem', background: 'var(--muted)', borderRadius: 'var(--radius)', display: 'flex' },
    title: { selector: '.action-step-title', fontSize: '1.5rem', fontWeight: '700', margin: '0' },
    subtitle: { selector: '.action-step-subtitle', color: 'var(--secondary)', margin: '0' },
    summaryBox: { selector: '.action-step-summary', background: 'var(--muted)', padding: '1.5rem', borderRadius: 'var(--radius)', marginBottom: '2.5rem', display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '1rem' },
    summaryLabel: { selector: '.action-step-summary-label', fontSize: '0.75rem', fontWeight: '600', color: 'var(--secondary)', textTransform: 'uppercase' },
    summaryValue: { selector: '.action-step-summary-value', fontWeight: '700' },
    errorBox: { selector: '.action-step-error', padding: '1rem', background: '#fef2f2', border: '1px solid #fee2e2', color: '#b91c1c', borderRadius: 'var(--radius)', marginBottom: '1.5rem', fontSize: '0.875rem' },
    actionsGrid: { selector: '.action-step-grid', display: 'grid', gap: '1.5rem' },
    quotesGrid: { selector: '.action-step-quotes-grid', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' },
    directChargeCard: { selector: '.action-direct-card', padding: '1.5rem', border: '1px solid var(--border)', borderRadius: 'var(--radius)', cursor: 'pointer', display: 'flex', gap: '1rem', background: 'white', transition: 'all 0.2s ease' },
    quoteBtnInner: { selector: '.action-quote-btn-inner', textAlign: 'center', display: 'flex', flexDirection: 'column', gap: '0.75rem', alignItems: 'center' },
    footer: { selector: '.action-step-footer', marginTop: '2.5rem' },
};
