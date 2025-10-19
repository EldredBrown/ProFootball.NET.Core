import { emptyStringErrorMessage, renderDetails } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";
import { League } from "../../data/models/league.js";

let seasonsCache = null;

const loadLeagueDetails = async (id) => {
    let league = await getData(`Leagues/${id}`);
    renderDetails("#league-details", league, "#league");
};

const loadSeasons = async () => {
    seasonsCache = await getData("Seasons");

    if (seasonsCache) {
        renderSeasons("first");
        renderSeasons("last");
    }
};

const renderSeasons = (firstOrLast) => {
    $(`#${firstOrLast}-season option`).remove();

    let template = $(`#${firstOrLast}-season-year`).html();
    let templateScript = Handlebars.compile(template);

    let seasons = null;
    switch (firstOrLast) {
        case "first":
            seasons = seasonsCache;
            break;

        case "last":
            seasons = seasonsCache.filter(s => s.year >= $("#first-season").val());
            seasons.unshift(null);
            break;

        default:
            break;
    }

    let context = {
        "seasons": seasons
    };

    let html = templateScript(context);
    $(`#${firstOrLast}-season`).append(html);
};

const validateInput = (leagueId = 0) => {
    let inputValid = true;

    $("#validation-summary li").remove();

    let longName = $("#long-name").val();
    if (longName) {
        $("#validation-for-long-name").text("");
    } else {
        $("#validation-for-long-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a long name.</li>");
        inputValid = false;
    }

    let shortName = $("#short-name").val();
    if (shortName) {
        $("#validation-for-short-name").text("");
    } else {
        $("#validation-for-short-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a short name.</li>");
        inputValid = false;
    }

    let firstSeasonYear = parseInt($("#first-season").val());
    if (firstSeasonYear) {
        $("#validation-for-first-season").text("");
    } else {
        let errorMessage = "Please enter a first season.";
        $("#validation-for-first-season").text(errorMessage);
        $("#validation-summary").append(`<li>${errorMessage}</li>`);
        inputValid = false;
    }

    let lastSeasonYear = parseInt($("#last-season").val());

    if (!inputValid) {
        return null;
    }

    return new League(leagueId, longName, shortName, firstSeasonYear, lastSeasonYear);
};

$("#first-season").change(function (e) {
    e.preventDefault();

    renderSeasons("last");
});

export { loadLeagueDetails, loadSeasons, renderSeasons, validateInput };
