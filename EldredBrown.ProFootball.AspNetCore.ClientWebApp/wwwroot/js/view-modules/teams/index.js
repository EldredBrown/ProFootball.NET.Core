import { getData } from "../../data/repository.js";

const loadPage = async () => {
    await loadTeams();
};

const loadTeams = async () => {
    let teams = await getData("Teams");
    if (teams) {
        renderTeams(teams);
    }
};

const renderTeams = (data) => {
    $("#teams-body tr").remove();

    let template = $("#team-row").html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "teams": data
    };

    let html = templateScript(context);
    $("#teams-body").append(html);
};

loadPage();
