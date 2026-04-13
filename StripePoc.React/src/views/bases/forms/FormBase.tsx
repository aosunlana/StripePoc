// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface FormBaseProps extends React.FormHTMLAttributes<HTMLFormElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLFormElement>;
}

const FormBase: React.FC<FormBaseProps> = (props) => (
    <Primitive as="form" {...props} />
);

export default FormBase;
