import { getRoute } from "../../utils/get-route";

const fileRoutes = {
    fetchFiles: (fileId=null, ownerId=null) => getRoute(`files/?parent=${fileId}&owner=${ownerId}`),
    fetchFile: (fileId) => getRoute(`files/${fileId}`),
    fetchFileBreadcrumbs: (fileId) => getRoute(`files/breadcrumbs/${fileId}`)
};

export default fileRoutes;