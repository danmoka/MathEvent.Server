const fileTypes = {
    application: {
      values: [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".zip", ".7z"],
      types: "application/pdf, application/zip, application/x-7z-compressed, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/vnd.ms-powerpoint, application/vnd.openxmlformats-officedocument.presentationml.presentation, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    },
    image: {
      values: [".jpg", ".jpeg", ".png", ".bmp"],
      types: "image/jpg, image/jpeg, image/png, image/bmp"
    },
    text: {
      values: [".csv"],
      types: ".csv"
    }
};

export default fileTypes;