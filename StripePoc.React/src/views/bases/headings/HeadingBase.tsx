// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface HeadingBaseProps {
    level: 1 | 2 | 3 | 4 | 5 | 6;
    text: string;
    className?: string;
    id?: string;
    style?: React.CSSProperties;
}

const HeadingBase: React.FC<HeadingBaseProps> = ({ level, text, ...props }) => {
    const Tag = `h${level}` as any; 
    return (
        <Primitive as={Tag} {...props}>
            {text}
        </Primitive>
    );
};

export default HeadingBase;
