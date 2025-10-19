import { loadSeasons } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";

let selectedSeasonYear = 1920;

const loadPage = async () => {
    await loadSeasons(selectedSeasonYear);
    await loadSeasonStandings();
};

const loadSeasonStandings = async () => {
    let seasonStandings = await getData(`SeasonStandings/${selectedSeasonYear}`);
    if (seasonStandings) {
        renderSeasonStandings(seasonStandings);
    }
};

const renderSeasonStandings = (seasonStandings) => {
    $("#season-standings-body tr").remove();

    let template = $("#season-standings-row").html();
    let templateScript = Handlebars.compile(template);

    let context = {
        "seasonStandings": seasonStandings
    };

    let html = templateScript(context);
    $("#season-standings-body").append(html);
};

$("#season").change(function (e) {
    e.preventDefault();
    selectedSeasonYear = parseInt($(this).val());
    loadSeasonStandings();
});

loadPage();
