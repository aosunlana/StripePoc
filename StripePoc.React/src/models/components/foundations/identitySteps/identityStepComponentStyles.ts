// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const IdentityStepComponentStyles = {
    container: { selector: '.identity-step-container', display: 'grid', gap: '1.5rem' },
    header: { selector: '.identity-step-header', display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '2rem' },
    iconWrap: { selector: '.identity-step-icon-wrap', padding: '0.75rem', background: 'var(--muted)', borderRadius: 'var(--radius)', display: 'flex', alignItems: 'center' },
    title: { selector: '.identity-step-title', fontSize: '1.5rem', fontWeight: '700', margin: '0' },
    subtitle: { selector: '.identity-step-subtitle', color: 'var(--secondary)', margin: '0' },
    fieldGroup: { selector: '.identity-step-field-group', display: 'grid', gap: '0.5rem' },
    label: { selector: '.identity-step-label', fontWeight: '600', fontSize: '0.875rem' },
    error: { selector: '.identity-step-error', color: '#ef4444', fontSize: '0.875rem', padding: '0.75rem', background: '#fef2f2', borderRadius: 'var(--radius)', border: '1px solid #fee2e2' },
    footer: { selector: '.identity-step-footer', marginTop: '2.5rem', display: 'flex', justifyContent: 'flex-end' },
};
