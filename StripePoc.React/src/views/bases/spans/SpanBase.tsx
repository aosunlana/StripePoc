// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface SpanBaseProps extends React.HTMLAttributes<HTMLSpanElement> {
    as?: React.ElementType;
    text?: string;
    componentRef?: React.Ref<HTMLSpanElement>;
}

const SpanBase: React.FC<SpanBaseProps> = ({ text, children, ...props }) => (
    <Primitive as="span" {...props}>
        {text}
        {children}
    </Primitive>
);

export default SpanBase;
