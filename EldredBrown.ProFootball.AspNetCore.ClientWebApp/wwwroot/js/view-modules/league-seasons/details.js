import { getIdParam } from "../../control-modules/site.js";
import { loadLeagueSeasonDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadLeagueSeasonDetails(id);
};

loadPage();
