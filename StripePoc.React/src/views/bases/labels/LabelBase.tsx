// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface LabelBaseProps extends React.LabelHTMLAttributes<HTMLLabelElement> {
    as?: React.ElementType;
    text?: string;
    componentRef?: React.Ref<HTMLLabelElement>;
}

const LabelBase: React.FC<LabelBaseProps> = ({ text, children, ...props }) => (
    <Primitive as="label" {...props}>
        {text}
        {children}
    </Primitive>
);

export default LabelBase;
