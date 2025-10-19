import { getIdParam } from "../../control-modules/site.js";
import { getData, putData } from "../../data/repository.js";
import { validateInput } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadTeamDetails();
};

const loadTeamDetails = async () => {
    let team = await getData(`Teams/${id}`);
    renderTeamDetails(team);
};

const renderTeamDetails = (team) => {
    $("#name").val(team.name);
};

const updateTeam = async () => {
    let team = validateInput(id);

    if (!team) {
        return;
    }

    await putData(`Teams/${id}`, team);

    return team;
};

$("form").submit(async function (e) {
    e.preventDefault();

    let team = await updateTeam();
    if (team) {
        window.location.pathname = "Teams/Index";
    }
});

loadPage();
