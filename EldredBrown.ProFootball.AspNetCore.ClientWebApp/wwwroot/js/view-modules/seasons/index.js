import { getData } from "../../data/repository.js";

const loadPage = async () => {
    await loadSeasons();
};

const loadSeasons = async () => {
    let seasons = await getData("Seasons");
    if (seasons) {
        renderSeasons(seasons);
    }
};

const renderSeasons = (data) => {
    $("#seasons-body tr").remove();

    let template = $("#season-row").html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "seasons": data
    };

    let html = templateScript(context);
    $("#seasons-body").append(html);
};

loadPage();
