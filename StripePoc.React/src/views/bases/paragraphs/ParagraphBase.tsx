// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface ParagraphBaseProps extends React.HTMLAttributes<HTMLParagraphElement> {
    as?: React.ElementType;
    text?: string;
    componentRef?: React.Ref<HTMLParagraphElement>;
}

const ParagraphBase: React.FC<ParagraphBaseProps> = ({ text, children, ...props }) => (
    <Primitive as="p" {...props}>
        {text}
        {children}
    </Primitive>
);

export default ParagraphBase;
