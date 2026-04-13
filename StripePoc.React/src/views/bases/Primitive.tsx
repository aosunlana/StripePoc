// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';

type PrimitiveProps<T extends React.ElementType> = {
    as?: T;
    children?: React.ReactNode;
    componentRef?: React.Ref<any>; // Using any for polymorphic ref compatibility across SVG and HTML
} & React.ComponentPropsWithoutRef<T>;

const Primitive = <T extends React.ElementType = 'div'>({
    as,
    children,
    componentRef,
    ...props
}: PrimitiveProps<T>) => {
    const Component = (as || 'div') as React.ElementType;
    return <Component ref={componentRef} {...props}>{children}</Component>;
};

export default Primitive;
