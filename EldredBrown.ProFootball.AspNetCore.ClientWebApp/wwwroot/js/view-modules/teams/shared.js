import { emptyStringErrorMessage, renderDetails } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";
import { Team } from "../../data/models/team.js";

const loadTeamDetails = async (id) => {
    let team = await getData(`Teams/${id}`);
    renderDetails("#team-details", team, "#team");
};

const validateInput = (teamId = 0) => {
    let inputValid = true;

    $("#validation-summary li").remove();

    let name = $("#name").val();
    if (name) {
        $("#validation-for-name").text("");
    } else {
        $("#validation-for-name").text(emptyStringErrorMessage);
        $("#validation-summary").append("<li>Please enter a name.</li>");
        inputValid = false;
    }

    if (!inputValid) {
        return null;
    }

    return new Team(teamId, name);
};

export { loadTeamDetails, validateInput };
