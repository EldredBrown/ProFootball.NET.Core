import { GamesControl } from "../../control-modules/games.js";
import { postData } from "../../data/repository.js";
import { loadPartial, validateInput } from "./shared.js";

const loadPage = async () => {
    GamesControl.getCookieValues();

    await loadPartial();
    selectWeek();
};

const createGame = async () => {
    let game = validateInput();

    if (!game) {
        return;
    }

    await postData("Games", game);

    return game;
};

const selectWeek = () => {
    if (GamesControl.selectedWeek) {
        $("#week").val(GamesControl.selectedWeek);
    } else {
        $("#week").val(1);
    }
};

$("form").submit(async function (e) {
    e.preventDefault();

    let game = await createGame();
    if (game) {
        GamesControl.setSelectedSeasonYear(game.seasonYear);
        GamesControl.setSelectedWeek(game.week);

        window.location.pathname = "Games/Create";
    }
});

loadPage();
