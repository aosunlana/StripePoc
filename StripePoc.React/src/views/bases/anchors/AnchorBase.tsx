// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface AnchorBaseProps extends React.AnchorHTMLAttributes<HTMLAnchorElement> {
    as?: React.ElementType;
    cssClass?: string;
    text?: string;
    componentRef?: React.Ref<HTMLAnchorElement>;
}

const AnchorBase: React.FC<AnchorBaseProps> = ({ cssClass, text, children, ...props }) => (
    <Primitive as="a" className={cssClass} {...props}>
        {text}
        {children}
    </Primitive>
);

export default AnchorBase;
