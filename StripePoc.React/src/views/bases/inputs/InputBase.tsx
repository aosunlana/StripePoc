// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface InputBaseProps extends React.InputHTMLAttributes<HTMLInputElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLInputElement>;
}

const InputBase: React.FC<InputBaseProps> = (props) => (
    <Primitive as="input" {...props} />
);

export default InputBase;
