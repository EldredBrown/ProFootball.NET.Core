import { getIdParam, loadSeasons } from "../../control-modules/site.js";
import { getData, putData } from "../../data/repository.js";
import { validateInput } from "./shared.js";

const id = getIdParam();

const loadLeagueSeasonDetails = async () => {
    let leagueSeason = await getData(`LeagueSeasons/${id}`);
    renderLeagueSeasonDetails(leagueSeason);
};

const loadPage = async () => {
    await loadSeasons();
    await loadLeagueSeasonDetails();
};

const renderLeagueSeasonDetails = (leagueSeason) => {
    $("#league-name").val(leagueSeason.leagueName);
    $("#season").val(leagueSeason.seasonYear);
    $("#total-games").val(leagueSeason.totalGames);
    $("#total-points").val(leagueSeason.totalPoints);
    $("#average-points").val(leagueSeason.averagePoints);
};

const updateLeagueSeason = async () => {
    let leagueSeason = validateInput(id);

    if (!leagueSeason) {
        return;
    }

    await putData(`LeagueSeasons/${id}`, leagueSeason);

    return leagueSeason;
};

$("form").submit(async function (e) {
    e.preventDefault();

    let leagueSeason = await updateLeagueSeason();
    if (leagueSeason) {
        window.location.pathname = "LeagueSeasons/Index";
    }
});

loadPage();
