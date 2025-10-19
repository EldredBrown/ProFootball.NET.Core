import { getIdParam } from "../../control-modules/site.js";
import { deleteData } from "../../data/repository.js";
import { loadLeagueDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadLeagueDetails(id);
};

$("form").submit(function (e) {
    e.preventDefault();
    deleteData(`Leagues/${id}`);
    window.location.pathname = "/Leagues/Index";
});

loadPage();
