export class Season {
    constructor(id, year, numOfWeeksScheduled = 0, numOfWeeksCompleted = 0) {
        this.id = id;
        this.year = year;
        this.numOfWeeksScheduled = numOfWeeksScheduled;
        this.numOfWeeksCompleted = numOfWeeksCompleted;
    }
}
