import { getIdParam } from "../../control-modules/site.js";
import { loadTeamDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadTeamDetails(id);
};

loadPage();
