import { getIdParam } from "../../control-modules/site.js";
import { loadLeagueDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadLeagueDetails(id);
};

loadPage();
