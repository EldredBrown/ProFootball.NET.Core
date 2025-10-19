export class League {
    constructor(id, longName, shortName, firstSeasonYear, lastSeasonYear = null) {
        this.id = id;
        this.longName = longName;
        this.shortName = shortName;
        this.firstSeasonYear = firstSeasonYear;
        this.lastSeasonYear = lastSeasonYear;
    }
}
