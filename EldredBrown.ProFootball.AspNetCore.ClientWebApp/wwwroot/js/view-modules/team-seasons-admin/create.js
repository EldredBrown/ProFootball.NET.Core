import { postData } from "../../data/repository.js";
import { loadPartial, validateInput } from "./shared.js";

const createTeamSeason = async () => {
    let teamSeason = validateInput();

    if (!teamSeason) {
        return;
    }

    await postData("TeamSeasons", teamSeason);

    return teamSeason;
};

const loadPage = async () => {
    loadPartial();
};

$("form").submit(async function (e) {
    e.preventDefault();

    let teamSeason = await createTeamSeason();
    if (teamSeason) {
        window.location.pathname = "TeamSeasonsAdmin/Index";
    }
});

loadPage();
