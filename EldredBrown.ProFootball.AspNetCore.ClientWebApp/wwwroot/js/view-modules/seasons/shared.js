import { renderDetails } from "../../control-modules/site.js";
import { getData } from "../../data/repository.js";
import { Season } from "../../data/models/season.js";

let maxWeeksCompleted = 0;

const loadSeasonDetails = async (id) => {
    let season = await getData(`Seasons/${id}`);
    renderDetails("#season-details", season, "#season");
};

const validateInput = (seasonId = 0) => {
    let inputValid = true;

    $("#validation-summary li").remove();

    let year = parseInt($("#year").val());
    if (year) {
        $("#validation-for-year").text("");
    } else {
        let errorString = "Please enter a year.";
        $("#validation-for-year").text(errorString);
        $("#validation-summary").append(`<li>${errorString}</li>`);
        inputValid = false;
    }

    let numOfWeeksScheduled = parseInt($("#weeks-scheduled").val());
    let numOfWeeksCompleted = parseInt($("#weeks-completed").val());

    if (!inputValid) {
        return null;
    }

    return new Season(seasonId, year, numOfWeeksScheduled, numOfWeeksCompleted);
};

$("#weeks-scheduled").change(function (e) {
    e.preventDefault();

    maxWeeksCompleted = parseInt($(this).val());
    if (parseInt($("#weeks-completed").val()) > maxWeeksCompleted) {
        $("#weeks-completed").val(maxWeeksCompleted);
    }
    $("#weeks-completed").prop("max", maxWeeksCompleted);
});

export { loadSeasonDetails, validateInput };
