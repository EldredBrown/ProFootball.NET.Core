import { getIdParam } from "../../control-modules/site.js";
import { loadSeasonDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadSeasonDetails(id);
};

loadPage();
