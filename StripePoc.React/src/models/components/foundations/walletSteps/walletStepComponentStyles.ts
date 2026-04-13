// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const WalletStepComponentStyles = {
    container: { selector: '.wallet-step-container', display: 'grid', gap: '1rem' },
    header: { selector: '.wallet-step-header', display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' },
    headerLeft: { selector: '.wallet-step-header-left', display: 'flex', alignItems: 'center', gap: '1rem' },
    iconWrap: { selector: '.wallet-step-icon-wrap', padding: '0.75rem', background: 'var(--muted)', borderRadius: 'var(--radius)', display: 'flex' },
    title: { selector: '.wallet-step-title', fontSize: '1.5rem', fontWeight: '700', margin: '0' },
    subtitle: { selector: '.wallet-step-subtitle', color: 'var(--secondary)', margin: '0' },
    emptyState: { selector: '.wallet-step-empty', textAlign: 'center', padding: '3rem', border: '2px dashed var(--border)', borderRadius: 'var(--radius)', color: 'var(--secondary)' },
    footer: { selector: '.wallet-step-footer', marginTop: '2.5rem', display: 'flex', justifyContent: 'space-between' },
    methodCard: { selector: '.wallet-method-card', padding: '1rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center', border: '1px solid var(--border)', borderRadius: 'var(--radius)', background: 'white' },
    methodInfo: { selector: '.wallet-method-info', display: 'flex', alignItems: 'center', gap: '1rem' },
    methodName: { selector: '.wallet-method-name', fontWeight: '600' },
    methodStatus: { selector: '.wallet-method-status', fontSize: '0.75rem', color: 'var(--secondary)' },
};
