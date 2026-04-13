// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface ButtonBaseProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    as?: React.ElementType;
    text?: string;
    componentRef?: React.Ref<HTMLButtonElement>;
}

const ButtonBase: React.FC<ButtonBaseProps> = ({ text, children, type = 'button', ...props }) => (
    <Primitive as="button" type={type} {...props}>
        {text}
        {children}
    </Primitive>
);

export default ButtonBase;
