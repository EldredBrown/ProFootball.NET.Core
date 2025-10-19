import { getIdParam } from "../../control-modules/site.js";
import { getData, putData } from "../../data/repository.js";
import { loadPartial, validateInput } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadPartial();
    await loadTeamSeasonDetails();
};

const loadTeamSeasonDetails = async () => {
    let teamSeason = await getData(`TeamSeasons/${id}`);
    renderTeamSeasonDetails(teamSeason);
};

const renderTeamSeasonDetails = (teamSeason) => {
    $("#team-name").val(teamSeason.teamName);
    $("#season").val(teamSeason.seasonYear);
    $("#league-name").val(teamSeason.leagueName);
};

const updateTeamSeason = async () => {
    let teamSeason = validateInput(id);

    if (!teamSeason) {
        return;
    }

    await putData(`TeamSeasons/${id}`, teamSeason);

    return teamSeason;
};

$("form").submit(async function (e) {
    e.preventDefault();

    let teamSeason = await updateTeamSeason();
    if (teamSeason) {
        window.location.pathname = "TeamSeasonsAdmin/Index";
    }
});

loadPage();
