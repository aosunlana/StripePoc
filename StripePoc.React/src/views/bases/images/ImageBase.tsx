// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import React from 'react';
import Primitive from '../Primitive';

interface ImageBaseProps extends React.ImgHTMLAttributes<HTMLImageElement> {
    as?: React.ElementType;
    componentRef?: React.Ref<HTMLImageElement>;
}

const ImageBase: React.FC<ImageBaseProps> = (props) => (
    <Primitive as="img" {...props} />
);

export default ImageBase;
