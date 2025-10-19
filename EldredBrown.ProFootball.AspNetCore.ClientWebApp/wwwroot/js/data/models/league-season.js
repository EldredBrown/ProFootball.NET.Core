export class LeagueSeason {
    constructor(id, leagueName, seasonYear, totalGames = 0, totalPoints = 0, averagePoints = null) {
        this.id = id;
        this.leagueName = leagueName;
        this.seasonYear = seasonYear;
        this.totalGames = totalGames;
        this.totalPoints = totalPoints;
        this.averagePoints = averagePoints;
    }
}
