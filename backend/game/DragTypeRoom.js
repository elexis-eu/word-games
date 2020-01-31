import room from "./Room";

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

class DragTypeRoom extends room.Room {

    constructor(query, game_conf) {
        super(query, game_conf, "drag");

        this.number_of_rounds = this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_CYCLE;
        this.round_pause_duration_ms = this.GameConfig.DRAG_ROUND_PAUSE_DURATION;
        this.buttons_number = this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND;
        this.round_duration_ms = this.GameConfig.DRAG_DURATION_OF_ROUND;
        // this.collecting_results_duration_ms = this.GameConfig.DURATION_WAIT_FOR_SCORES;
        // this.show_results_duration_ms = this.GameConfig.DURATION_SHOW_SCORES;
        this.scoring = this.GameConfig.DRAG_SCORING;
    }

    async start_game() {
        let delay = this.next_cycle_start - new Date().getTime();
        console.log(this.room_type_id, " Delay: ", delay / 1000.0);
        console.log(this.room_type_id, this.next_cycle_start);
        await sleep(delay);
        console.log(this.room_type_id, " Game Starts now: ", new Date());

        // Game loop
        while (true) {
            this.next_play_state();
            await sleep(this.GameConfig.DRAG_ROUNDS_DURATION - this.connect_immediate_lock_time);
            this.lock_immediate = true;
            await sleep(this.connect_immediate_lock_time);
            this.lock_immediate = false;
            this.wait_for_scores_state();
            await sleep(this.GameConfig.DURATION_WAIT_FOR_SCORES);
            this.show_scores_state();
            let delay = this.next_cycle_start - new Date().getTime();
            console.log(this.room_type_id, " Delay: ", delay / 1000.0);
            console.log(this.room_type_id, this.next_cycle_start);
            await sleep(delay);
        }
    }

    initialize_game() {
        this.initialize_cycle();
        setTimeout(this.start_game.bind(this), 1000);
    }

    next_play_state() {
        super.next_play_state();
        console.log(this.room_type_id, "new game " + room.Room.date_to_string(new Date()));
        this.initialize_cycle();
    }

    get_global_scoreboard(user_id) {
        return new Promise(resolve => {
            room.Room.get_global_scoreboard_func(user_id,
                'drag_score',
                this.query.get_drag_scoreboard.bind(this.query),
                this.query.get_drag_score.bind(this.query),
                resolve);
        });
    }
}


module.exports = DragTypeRoom;