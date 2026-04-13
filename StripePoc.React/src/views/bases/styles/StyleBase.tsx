// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';

interface StyleBaseProps {
    styles: Record<string, Record<string, string>>;
    componentRef?: React.Ref<HTMLStyleElement>;
}

const toCssProperty = (camelCase: string): string =>
    camelCase.replace(/([A-Z])/g, (m) => `-${m.toLowerCase()}`);

const objectToCss = (selector: string, styleObj: Record<string, string>): string => {
    const props = Object.entries(styleObj)
        .map(([k, v]) => `  ${toCssProperty(k)}: ${v};`)
        .join('\n');
    return `${selector} {\n${props}\n}`;
};

const StyleBase: React.FC<StyleBaseProps> = ({ styles, componentRef }) => {
    const css = Object.values(styles)
        .map(({ selector, ...props }) => objectToCss(selector, props as Record<string, string>))
        .join('\n\n');

    return <style ref={componentRef}>{css}</style>;
};

export default StyleBase;
