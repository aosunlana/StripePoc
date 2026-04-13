// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface DivisionBaseProps extends React.HTMLAttributes<HTMLDivElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLDivElement>;
}

const DivisionBase: React.FC<DivisionBaseProps> = (props) => (
    <Primitive as="div" {...props} />
);

export default DivisionBase;
