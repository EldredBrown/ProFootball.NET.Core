import { emptyStringErrorMessage, loadSeasons, renderDetails } from "../../control-modules/site.js";
import { GamesControl, loadWeeks } from "../../control-modules/games.js";
import { getData } from "../../data/repository.js";
import { Game } from "../../data/models/game.js";

const loadGameDetails = async (id) => {
    let game = await getData(`Games/${id}`);
    renderDetails("#game-details", game, "#game");
    $("#is-playoff-game")[0].checked = game.isPlayoffGame;
};

const loadPartial = async () => {
    await loadSeasons(GamesControl.selectedSeasonYear);
    await loadWeeks(renderWeeks);
};

const renderWeeks = (numOfWeeksScheduled) => {
    $("#week option").remove();

    let template = $("#week-value").html();
    let templateScript = Handlebars.compile(template);

    let weeks = [];
    for (var i = 1; i <= numOfWeeksScheduled; i++) {
        weeks.push({ "week": i });
    }

    let context = {
        "weeks": weeks
    };

    let html = templateScript(context);
    $("#week").append(html);
};

const validateInput = (gameId = 0) => {
    let inputValid = true;

    $("#validation-summary li").remove();

    let seasonYear = parseInt($("#season").val());
    if (seasonYear) {
        $("#validation-for-season-year").text("");
    } else {
        let errorMessage = "Please enter a season.";
        $("#validation-for-season-year").text(errorMessage);
        $("#validation-summary").append(`<li>${errorMessage}</li>`);
        inputValid = false;
    }

    let week = parseInt($("#week").val());
    if (week) {
        $("#validation-for-week").text("");
    } else {
        let errorMessage = "Please enter a week.";
        $("#validation-for-week").text(errorMessage);
        $("#validation-summary").append(`<li>${errorMessage}</li>`);
        inputValid = false;
    }

    let guestName = $("#guest-name").val();
    if (guestName) {
        $("#validation-for-guest-name").text("");
    } else {
        $("#validation-for-guest-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a guest.</li>");
        inputValid = false;
    }

    let guestScore = parseInt($("#guest-score").val());
    if (guestScore >= 0) {
        $("#validation-for-guest-score").text("");
    } else {
        let errorMessage = "Please enter the guest's score.";
        $("#validation-for-guest-score").text(errorMessage);
        $("#validation-summary").append(`<li>${errorMessage}</li>`);
        inputValid = false;
    }

    let hostName = $("#host-name").val();
    if (hostName) {
        $("#validation-for-host-name").text("");
    } else {
        $("#validation-for-host-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a host.</li>");
        inputValid = false;
    }

    let hostScore = parseInt($("#host-score").val());
    if (hostScore >= 0) {
        $("#validation-for-host-score").text("");
    } else {
        let errorMessage = "Please enter the host's score.";
        $("#validation-for-host-score").text(errorMessage);
        $("#validation-summary").append(`<li>${errorMessage}</li>`);
        inputValid = false;
    }

    let isPlayoffGame = $("#is-playoff-game")[0].checked;
    let notes = $("#notes").val();

    if (!inputValid) {
        return null;
    }

    return new Game(gameId, seasonYear, week, guestName, guestScore, hostName, hostScore, isPlayoffGame, notes);
};

export { loadGameDetails, loadPartial, renderWeeks, validateInput };
