import { seasons } from "./site.js";

class GamesControl {
    static selectedSeasonYear = 1920;
    static selectedWeek = null;

    static getCookieValues() {
        let decodedCookie = decodeURIComponent(document.cookie);

        if (decodedCookie) {
            let cookieStrings = decodedCookie.split("; ");

            let cookies = [];
            cookieStrings.forEach(cookieString => {
                let carr = cookieString.split("=");
                let cname = carr[0];
                let cvalue = carr[1];
                let cookie = { "cname": cname, "cvalue": cvalue };
                cookies.push(cookie);
            });

            let selectedSeasonYearString = cookies.find(cookie => cookie.cname === "selectedSeasonYear").cvalue;
            GamesControl.selectedSeasonYear = parseInt(selectedSeasonYearString);

            let selectedWeekString = cookies.find(cookie => cookie.cname === "selectedWeek").cvalue;
            GamesControl.selectedWeek = parseInt(selectedWeekString);
        }
    }

    static setCookie(seasonYear = null, week = null) {
        document.cookie = `selectedSeasonYear=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;`;
        document.cookie = `selectedWeek=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;`;

        if (seasonYear) {
            document.cookie = `selectedSeasonYear=${seasonYear}; path=/;`;
        } else {
            document.cookie = `selectedSeasonYear=${GamesControl.selectedSeasonYear}; path=/;`;
        }

        if (week) {
            document.cookie = `selectedWeek=${week}; path=/;`;
        } else {
            document.cookie = `selectedWeek=${GamesControl.selectedWeek}; path=/;`;
        }
    }

    static setSelectedSeasonYear(seasonYear) {
        GamesControl.selectedSeasonYear = seasonYear;
        GamesControl.setCookie(seasonYear, null);
    }

    static setSelectedWeek(week) {
        GamesControl.selectedWeek = week;
        GamesControl.setCookie(null, week);
    }
}

const loadWeeks = async (renderFunc) => {
    let season = seasons.filter(element => element.year === GamesControl.selectedSeasonYear)[0];
    renderFunc(season.numOfWeeksScheduled);
};

export { GamesControl, loadWeeks };
