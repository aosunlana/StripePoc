// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface OptionBaseProps extends React.OptionHTMLAttributes<HTMLOptionElement> {
    as?: React.ElementType;
    text?: string;
    componentRef?: React.Ref<HTMLOptionElement>;
}

const OptionBase: React.FC<OptionBaseProps> = ({ text, children, ...props }) => (
    <Primitive as="option" {...props}>
        {text}
        {children}
    </Primitive>
);

export default OptionBase;
