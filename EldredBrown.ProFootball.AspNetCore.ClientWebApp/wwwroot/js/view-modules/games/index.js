import { loadSeasons } from "../../control-modules/site.js";
import { GamesControl, loadWeeks } from "../../control-modules/games.js";
import { getData } from "../../data/repository.js";

let gamesCache = null;

const loadPage = async () => {
    GamesControl.getCookieValues();

    await loadSeasons(GamesControl.selectedSeasonYear);
    await loadWeeks(renderWeeks);
    await loadGames();
};

const loadGames = async () => {
    if (!gamesCache) {
        gamesCache = await getData("Games");
    }

    if (gamesCache) {
        let games = gamesCache.filter(g => g.seasonYear === GamesControl.selectedSeasonYear);

        if (GamesControl.selectedWeek) {
            games = games.filter(g => g.week === GamesControl.selectedWeek);
        }

        renderGames(games);
    }
};

const renderGames = (games) => {
    $("#games-body tr").remove();

    let template = $("#game-row").html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "games": games
    };

    let html = templateScript(context);
    $("#games-body").append(html);
};

const renderWeeks = (numOfWeeksScheduled) => {
    $("#week option").remove();

    let template = $("#week-value").html();
    let templateScript = Handlebars.compile(template);

    let weeks = [];
    for (var i = 0; i <= numOfWeeksScheduled; i++) {
        if (i === 0) {
            weeks.push({ "week": null });
        } else {
            weeks.push({ "week": i });
        }
    }

    let context = {
        "weeks": weeks
    };

    let html = templateScript(context);
    $("#week").append(html);

    $("#week").val(GamesControl.selectedWeek);
};

$("#season").change(async function (e) {
    e.preventDefault();

    let year = parseInt($(this).val());
    GamesControl.setSelectedSeasonYear(year);
    GamesControl.setSelectedWeek(null);

    await loadWeeks(renderWeeks);
    await loadGames();
});

$("#week").change(async function (e) {
    e.preventDefault();

    let week = parseInt($(this).val());

    if (week) {
        GamesControl.setSelectedWeek(week);
    } else {
        GamesControl.setSelectedWeek(null);
    }

    await loadGames();
});

loadPage();
