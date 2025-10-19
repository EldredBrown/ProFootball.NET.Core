import { getData } from "../../data/repository.js";

const guestOrHost = {
    guest: 1,
    host: 2
};

const firstYear = 1920;

let teamSeasonsCache = null;

const loadPage = async () => {
    await loadSeasons();

    await loadTeamNames(guestOrHost.guest);
    await loadTeamNames(guestOrHost.host);
};

const loadSeasons = async () => {
    let seasons = await getData("Seasons");

    if (seasons) {
        renderSeasons(guestOrHost.guest, seasons);
        renderSeasons(guestOrHost.host, seasons);
    }
};

const loadTeamNames = async (guestOrHost, year = firstYear) => {
    if (!teamSeasonsCache) {
        teamSeasonsCache = await getData("TeamSeasons");
    }

    let teamSeasons = teamSeasonsCache.filter(ts => ts.seasonYear === year);
    renderTeamNames(guestOrHost, teamSeasons);
};

const renderSeasons = (team, seasons) => {
    let templateTag;
    let selectTag;

    switch (team) {
        case guestOrHost.guest:
            templateTag = $("#guest-season-year");
            selectTag = $("#guest-season");
            break;

        case guestOrHost.host:
            templateTag = $("#host-season-year");
            selectTag = $("#host-season");
            break;

        default:
            break;
    }

    let template = templateTag.html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "seasons": seasons
    };

    let html = templateScript(context);
    selectTag.append(html);

    selectTag.val(firstYear);
};

const renderTeamNames = (team, teamSeasons) => {
    let templateTagId;
    let selectTagId;

    switch (team) {
        case guestOrHost.guest:
            templateTagId = "#guest";
            selectTagId = "#guest-name";
            break;

        case guestOrHost.host:
            templateTagId = "#host";
            selectTagId = "#host-name";
            break;

        default:
            break;
    }

    $(`${selectTagId} option`).remove();

    let template = $(templateTagId).html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "teamSeasons": teamSeasons
    };

    let html = templateScript(context);
    $(selectTagId).append(html);
};

$("form").submit(function (e) {
    e.preventDefault();

    // 2020-01-31: This is stubbed for now. I will eventually want to calculate the prediction and post it to the form.
    $("#guest-score").text(0);
    $("#host-score").text(0);
});

$(".season-select").change(async function (e) {
    e.preventDefault();

    let year = parseInt($(this).val());

    switch ($(this)[0].id) {
        case "guest-season":
            await loadTeamNames(guestOrHost.guest, year);
            break;

        case "host-season":
            await loadTeamNames(guestOrHost.host, year);
            break;

        default:
            break;
    }
});

loadPage();
