import React, { useCallback } from "react";
import { useDropzone } from "react-dropzone";
import Box from "@material-ui/core/Box";
import Typography from "@material-ui/core/Typography";
import { Icon, iconTypes } from "../Icon";
import "./FileDropzone.scss";

function fileValidator(file) {
    const maxNameLength = 20;
    const maxSize = 2097152;

    if (file.name.length > maxNameLength) {
        return {
            code: "name-too-large",
            message: `Сократите имя файла до ${maxNameLength} символов`
        };
    }

    if (file.size > maxSize) {
        return {
            code: "size-too-large",
            message: `Превышен размер. Максимальный размер: ${maxSize} bytes`
        }
    }

    return null
  }

const FileDropzone = ({ acceptedFileTypes, acceptedFileValues, maxFiles = 5, onDrop }) => {
    const handleDrop = useCallback((files) => {
        onDrop(files);
    }, [onDrop]);

    const { acceptedFiles, fileRejections, getInputProps, getRootProps } = useDropzone({
        accept: acceptedFileTypes.join(", "),
        maxFiles,
        validator: fileValidator,
        onDrop: handleDrop,
    });

    const acceptedFileItems = acceptedFiles.map((file) => (
        <li key={file.path}>
            {file.path} - {file.size} bytes
        </li>
    ));

    const fileRejectionItems = fileRejections.map(({ file, errors }) => (
        <li key={file.path}>
            {file.path} - {file.size} bytes
            <ul>
                {errors.map((e) => (
                    <li key={e.code}>{e.message}</li>
                ))}
            </ul>
        </li>
    ));

    return (
        <div>
            <div {...getRootProps()}>
                <Box className="dropzone" border={1} borderColor="primary.main" borderRadius="borderRadius">
                    <input {...getInputProps()} />
                    <Typography variant="h6" gutterBottom>Перетащите файлы или кликните здесь</Typography>
                    <Icon type={iconTypes.uploadCloud}/>
                    {acceptedFileValues.map((values, index) => (
                        <Typography key={index} variant="overline" display="block">{values.join(" ")}</Typography>
                    ))}
                </Box>
            </div>
            <aside className="aside">
                <Typography variant="subtitle1" display="block">Выбранные файлы:</Typography>
                <ul>{acceptedFileItems}</ul>
                {fileRejectionItems.length > 0
                ? (
                    <>
                    <Typography variant="subtitle1">Не удалось выбрать: </Typography>
                    <ul>{fileRejectionItems}</ul>
                    </>)
                : (<></>)
                }
            </aside>
        </div>
    );
};

export default FileDropzone;