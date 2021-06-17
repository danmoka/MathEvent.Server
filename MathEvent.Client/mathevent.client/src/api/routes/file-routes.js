import { getRoute } from "../../utils/get-route";

const fileRoutes = {
    fetchFiles: (fileId=null, ownerId=null) => getRoute(`files/?parent=${fileId}&owner=${ownerId}`),
    fetchFile: (fileId) => getRoute(`files/${fileId}`),
    fetchFileBreadcrumbs: (fileId) => getRoute(`files/breadcrumbs/${fileId}`),
    createFile: () => getRoute(`files/`),
    deleteFile: (fileId) => getRoute(`files/${fileId}`),
    uploadFiles: (fileId, ownerId) => getRoute(`files/upload/?parentId=${fileId}&ownerId=${ownerId}`),
    downloadFile: (fileId) => getRoute(`files/download/${fileId}`),
};

export default fileRoutes;