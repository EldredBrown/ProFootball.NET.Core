export class Game {
    constructor(id, seasonYear, week, guestName, guestScore, hostName, hostScore, isPlayoffGame, notes = null) {
        this.id = id;
        this.seasonYear = seasonYear;
        this.week = week;
        this.guestName = guestName;
        this.guestScore = guestScore;
        this.hostName = hostName;
        this.hostScore = hostScore;
        this.isPlayoffGame = isPlayoffGame;
        this.notes = notes;
    }
}
