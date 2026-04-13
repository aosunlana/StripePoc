// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface LoaderBaseProps extends React.SVGAttributes<SVGSVGElement> {
    as?: React.ElementType;
    size?: number;
    color?: string;
    componentRef?: React.Ref<SVGSVGElement>;
}

const LoaderBase: React.FC<LoaderBaseProps> = ({ size = 24, color = 'currentColor', className, ...props }) => {
    return (
        <Primitive
            as="svg"
            width={size}
            height={size}
            viewBox="0 0 24 24"
            fill="none"
            stroke={color}
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            className={`animate-spin ${className || ''}`}
            {...props}
        >
            <path d="M21 12a9 9 0 1 1-6.219-8.56" />
        </Primitive>
    );
};

export default LoaderBase;
