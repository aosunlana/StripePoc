// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const OutcomeViewComponentStyles = {
    container: { selector: '.outcome-view-container', textAlign: 'center' },
    iconWrap: { selector: '.outcome-view-icon-wrap', width: '4rem', height: '4rem', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1.5rem' },
    title: { selector: '.outcome-view-title', fontSize: '1.75rem', fontWeight: '800', marginBottom: '0.5rem' },
    subtitle: { selector: '.outcome-view-subtitle', color: 'var(--secondary)', marginBottom: '2.5rem' },
    detailBox: { selector: '.outcome-view-detail', background: 'var(--background)', border: '1px solid var(--border)', borderRadius: 'var(--radius)', padding: '1.5rem', marginBottom: '2.5rem', textAlign: 'left' },
    detailGrid: { selector: '.outcome-view-detail-grid', display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '1.5rem' },
    detailLabel: { selector: '.outcome-view-detail-label', fontSize: '0.75rem', fontWeight: '600', color: 'var(--secondary)', textTransform: 'uppercase' },
    detailValue: { selector: '.outcome-view-detail-value', fontWeight: '700', fontSize: '0.875rem' },
    approvalLink: { selector: '.outcome-view-approval-link', width: '100%', gap: '0.75rem', padding: '1.25rem', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '0.75rem' },
    approvalNote: { selector: '.outcome-view-approval-note', fontSize: '0.75rem', color: 'var(--secondary)', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.25rem' },
    actionsRow: { selector: '.outcome-view-actions', display: 'flex', gap: '1rem' },
};
