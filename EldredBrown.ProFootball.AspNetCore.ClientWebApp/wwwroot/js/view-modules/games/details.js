import { getIdParam } from "../../control-modules/site.js";
import { loadGameDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadGameDetails(id);
};

loadPage();
