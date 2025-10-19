import { getIdParam } from "../../control-modules/site.js";
import { loadTeamSeasonDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadTeamSeasonDetails(id);
};

loadPage();
