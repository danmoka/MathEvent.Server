import React from "react";
import Image from "../Image";
import images from "../../../constants/images";

const FileImage = ({ className, ext, hierarchy, alt }) => {
    let src = hierarchy ? images.folder : images.basic_file;

    if (ext) {
        ext = ext.slice(1);
        const path = images[ext];
        src = path ? path : src;
    }

    return(
        <Image
            className={className}
            src={src}
            alt={alt}
        />
    );
};

export default FileImage;