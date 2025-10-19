import { emptyStringErrorMessage, renderDetails } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";
import { LeagueSeason } from "../../data/models/league-season.js";

const loadLeagueSeasonDetails = async (id) => {
    let leagueSeason = await getData(`LeagueSeasons/${id}`);
    renderDetails("#league-season-details", leagueSeason, "#league-season");
};

const validateInput = (leagueSeasonId = 0) => {
    let inputValid = true;

    $("#validation-summary li").remove();

    let leagueName = $("#league-name").val();
    if (leagueName) {
        $("#validation-for-league-name").text("");
    } else {
        $("#validation-for-league-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a league name.</li>");
        inputValid = false;
    }

    let seasonYear = parseInt($("#season").val());
    if (seasonYear) {
        $("#validation-for-season-year").text("");
    } else {
        $("#validation-for-season-year").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a year.</li>");
        inputValid = false;
    }

    let totalGames = parseInt($("#total-games").val());
    let totalPoints = parseInt($("#total-points").val());
    let averagePoints = parseFloat($("#average-points").val());

    if (!inputValid) {
        return null;
    }

    return new LeagueSeason(leagueSeasonId, leagueName, seasonYear, totalGames, totalPoints, averagePoints);
};

export { loadLeagueSeasonDetails, validateInput };
