// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface ClickableDivisionBaseProps extends React.HTMLAttributes<HTMLDivElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLDivElement>;
}

const ClickableDivisionBase: React.FC<ClickableDivisionBaseProps> = (props) => (
    <Primitive as="div" style={{ cursor: 'pointer' }} {...props} />
);

export default ClickableDivisionBase;
