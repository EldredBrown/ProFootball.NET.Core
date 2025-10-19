import { postData } from "../../data/repository.js";
import { validateInput } from "./shared.js";

const createSeason = async () => {
    let season = validateInput();

    if (!season) {
        return;
    }

    await postData("Seasons", season);

    return season;
};

$("form").submit(async function (e) {
    e.preventDefault();

    let season = await createSeason();
    if (season) {
        window.location.pathname = "Seasons/Index";
    }
});
