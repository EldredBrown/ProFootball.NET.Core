import { getIdParam } from "../../control-modules/site.js";
import { deleteData } from "../../data/repository.js";
import { loadSeasonDetails } from "./shared.js";

const id = getIdParam();

const loadPage = async () => {
    await loadSeasonDetails(id);
};

$("form").submit(function (e) {
    e.preventDefault();
    deleteData(`Seasons/${id}`);
    window.location.pathname = "/Seasons/Index";
});

loadPage();
