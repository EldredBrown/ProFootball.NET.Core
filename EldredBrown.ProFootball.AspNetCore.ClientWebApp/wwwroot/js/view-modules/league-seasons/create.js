import { loadSeasons } from "../../control-modules/site.js";
import { postData } from "../../data/repository.js";
import { validateInput } from "./shared.js";

const createLeagueSeason = async () => {
    let leagueSeason = validateInput();

    if (!leagueSeason) {
        return;
    }

    await postData("LeagueSeasons", leagueSeason);

    return leagueSeason;
};

const loadPage = async () => {
    await loadSeasons();
};

$("form").submit(async function (e) {
    e.preventDefault();

    let leagueSeason = await createLeagueSeason();
    if (leagueSeason) {
        window.location.pathname = "LeagueSeasons/Index";
    }
});

loadPage();
