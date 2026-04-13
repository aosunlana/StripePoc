// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface SelectBaseProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLSelectElement>;
}

const SelectBase: React.FC<SelectBaseProps> = (props) => (
    <Primitive as="select" {...props} />
);

export default SelectBase;
