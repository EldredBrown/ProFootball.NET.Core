import { getIdParam } from "../../control-modules/site.js";
import { deleteData } from "../../data/repository.js";
import { loadGameDetails } from "./shared.js";

const id = getIdParam();

const loadPage = () => {
    await loadGameDetails(id);
};

$("form").submit(function (e) {
    e.preventDefault();
    deleteData(`Games/${id}`);
    window.location.pathname = "/Games/Index";
});

loadPage();
