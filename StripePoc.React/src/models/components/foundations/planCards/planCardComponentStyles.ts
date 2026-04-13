// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

export const PlanCardComponentStyles = {
    card: { selector: '.plan-card', padding: '1.25rem', border: '2px solid var(--border)', borderRadius: 'var(--radius)', cursor: 'pointer', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: 'white', transition: 'all 0.2s ease' },
    cardSelected: { selector: '.plan-card.selected', borderColor: 'var(--primary)', backgroundColor: 'rgba(15, 23, 42, 0.02)' },
    name: { selector: '.plan-card-name', fontWeight: '700', fontSize: '1rem' },
    description: { selector: '.plan-card-description', fontSize: '0.875rem', color: 'var(--secondary)' },
    price: { selector: '.plan-card-price', fontWeight: '800', fontSize: '1.125rem', textAlign: 'right' },
    period: { selector: '.plan-card-period', fontSize: '0.75rem', color: 'var(--secondary)' },
};
