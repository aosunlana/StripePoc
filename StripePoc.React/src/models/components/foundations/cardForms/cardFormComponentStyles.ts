// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const CardFormComponentStyles = {
    container: { selector: '.card-form-container', display: 'grid', gap: '1.5rem' },
    header: { selector: '.card-form-header', display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' },
    headerTitle: { selector: '.card-form-title', fontWeight: '600', margin: '0' },
    cardInput: { selector: '.card-form-input-wrap', padding: '1rem', border: '1px solid var(--border)', borderRadius: 'var(--radius)', background: 'white' },
    errorMsg: { selector: '.card-form-error', color: '#ef4444', fontSize: '0.875rem' },
    addCardSection: { selector: '.card-form-section', background: 'var(--muted)', padding: '2rem', borderRadius: 'var(--radius)' },
};
