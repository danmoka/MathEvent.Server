  
import { getRoute } from "./get-route";

export const getImageSrc = (src) => getRoute(`images/?src=${src}`);