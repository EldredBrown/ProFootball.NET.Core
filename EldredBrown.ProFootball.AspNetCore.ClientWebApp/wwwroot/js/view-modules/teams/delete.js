import { getIdParam } from "../../control-modules/site.js";
import { deleteData } from "../../data/repository.js";
import { loadTeamDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadTeamDetails(id);
};

$("form").submit(function (e) {
    e.preventDefault();
    deleteData(`Teams/${id}`);
    window.location.pathname = "/Teams/Index";
});

loadPage();
