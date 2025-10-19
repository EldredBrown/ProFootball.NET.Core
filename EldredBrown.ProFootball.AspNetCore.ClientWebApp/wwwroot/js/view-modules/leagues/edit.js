import { getIdParam } from "../../control-modules/site.js";
import { getData, putData } from "../../data/repository.js";
import { loadSeasons, renderSeasons, validateInput } from "./shared.js";

const id = getIdParam();

const loadLeagueDetails = async () => {
    let league = await getData(`Leagues/${id}`);
    renderLeagueDetails(league);
};

const loadPage = async () => {
    await loadSeasons();
    await loadLeagueDetails(id, renderLeagueDetails);
};

const renderLeagueDetails = (league) => {
    $("#long-name").val(league.longName);
    $("#short-name").val(league.shortName);
    $("#first-season").val(league.firstSeasonYear);

    renderSeasons("last");
    $("#last-season").val(league.lastSeasonYear);
};

const updateLeague = async () => {
    let league = validateInput(id);

    if (!league) {
        return;
    }

    await putData(`Leagues/${id}`, league);

    return league;
};

$("form").submit(async function (e) {
    e.preventDefault();

    let league = await updateLeague();
    if (league) {
        window.location.pathname = "Leagues/Index";
    }
});

loadPage();
