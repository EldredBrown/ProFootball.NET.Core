import { getIdParam, renderDetails } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";

const id = getIdParam();

const loadPage = async () => {
    await loadTeamSeasonDetails(id);
};

const loadTeamSeasonDetails = async (id) => {
    let teamSeason = await getData(`TeamSeasons/${id}`);
    renderDetails("#team-season-details", teamSeason, "#team-season");
};

loadPage();
