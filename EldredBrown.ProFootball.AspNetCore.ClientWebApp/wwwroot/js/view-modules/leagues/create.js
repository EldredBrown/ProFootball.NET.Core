import { postData } from "../../data/repository.js";
import { loadSeasons, validateInput } from "./shared.js";

const createLeague = async () => {
    let league = validateInput();

    if (!league) {
        return;
    }

    await postData("Leagues", league);

    return league;
};

const loadPage = async () => {
    await loadSeasons();
};

$("form").submit(async function (e) {
    e.preventDefault();

    let league = await createLeague();
    if (league) {
        window.location.pathname = "Leagues/Index";
    }
});

loadPage();
