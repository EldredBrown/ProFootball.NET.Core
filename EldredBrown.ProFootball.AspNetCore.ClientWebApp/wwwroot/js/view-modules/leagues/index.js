import { getData } from "../../data/repository.js";

const loadLeagues = async () => {
    let leagues = await getData("Leagues");
    if (leagues) {
        renderLeagues(leagues);
    }
};

const loadPage = async () => {
    await loadLeagues();
};

const renderLeagues = (data) => {
    $("#leagues-body tr").remove();

    let template = $("#league-row").html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "leagues": data
    };

    let html = templateScript(context);
    $("#leagues-body").append(html);
};

loadPage();
