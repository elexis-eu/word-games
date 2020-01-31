import room from "./Room";


class ChooseTypeRoom extends room.Room {

    constructor(query, game_conf) {
        super(query, game_conf, "choose");
        this.number_of_rounds = this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE;
        this.max_select = this.GameConfig.MAX_SELECT;
        this.round_pause_duration_ms = this.GameConfig.ROUND_PAUSE_DURATION;
        this.buttons_number = this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND;
        this.round_duration_ms = this.GameConfig.CHOOSE_DURATION_OF_ROUND;
        this.collecting_results_duration_ms = this.GameConfig.DURATION_WAIT_FOR_SCORES;
        this.show_results_duration_ms = this.GameConfig.DURATION_SHOW_SCORES;
        this.scoring = this.GameConfig.CHOOSE_SCORING;
        this.bonus = this.GameConfig.CHOOSE_BONUS;
    }

    start_game() {
        console.log(this.room_type_id, "Start cycle", this.next_cycle_start);
        super.start_game().catch((err) => { throw err });
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

    static compare_order( a, b ) {
        if ( a.real < b.real ){
            return -1;
        }
        if ( a.real > b.real ){
            return 1;
        }
        return 0;
    }

    static choose_calculate_bonus(data, GameConfig, choose_positions) {
        // console.log("choose_calculate_bonus");
        let reward = {};
        // Check if correct order of words:
        let i;
        for (i = 0; i < data.length; i++) {
            let word = data[i].word;
            let is_reward = true;
            if (data[i].buttons.length != GameConfig.CHOOSE_NUMBER_OF_WORDS_TO_CHOOSE) {
                // console.log("Not all buttons selected: ", GameConfig.CHOOSE_NUMBER_OF_WORDS_TO_CHOOSE);
                // console.log("len: ", data[i].buttons.length);
                is_reward = false;
            }

            if (!is_reward)
                continue;

            let compare = [];
            let j;
            for (j = 0; j < data[i].buttons.length; j++) {
                if (data[i].buttons[j].position == null || data[i].buttons[j].position <= 0)  {
                    // console.log("data[i].buttons[j].position is null", data[i].buttons[j].position);
                    is_reward = false;
                }
                compare.push({
                    "predicted" : data[i].buttons[j].position,
                    "real" : choose_positions[word][data[i].buttons[j].word]
                });
            }

            if (!is_reward)
                continue;

            compare.sort(ChooseTypeRoom.compare_order);
            // console.log("compare: ", compare);

            let last_predicted = compare[0].predicted;
            for (j = 1; j < compare.length; j++) {
                // if (choose_positions[word][data[i].buttons[j].word] != data[i].buttons[j].position) {
                //     is_reward = false;
                //     break;
                // }
                // console.log("PREDICT: ", compare[j].predicted, " real: ", compare[j].real);
                if (last_predicted > compare[j].predicted) {
                    is_reward = false;
                    break;
                }
                last_predicted = compare[j].predicted;
            }
            reward[data[i].word] = 0;
            if (is_reward) {
                // console.log("REWARD!!!!");
                // console.log(compare);
                reward[data[i].word] = GameConfig.CHOOSE_BONUS;
            }
        }
        // console.log("Rewards: ", reward);
        return reward;
    }

    set_score_for_player(player_id, cycle_id, data) {
        let player = this.find_current_player(player_id);
        if (cycle_id != this.current_cycle_id || player == null || this.current_state == room.RoomState.SHOW_SCORES) {
            return null;
        }
        data = data.words;
        // console.log("Choose set score");
        // console.log(room.Room.print_words_obj(this.current_words));

        let reward = ChooseTypeRoom.choose_calculate_bonus(data, this.GameConfig, this.choose_positions);

        return player.update_scores(data, this.current_words_dict, reward);
    }


    get_global_scoreboard(user_id) {
        return new Promise(resolve => {
            room.Room.get_global_scoreboard_func(user_id,
                'choose_score',
                this.query.get_choose_scoreboard.bind(this.query),
                this.query.get_choose_score.bind(this.query),
                resolve);
        });
    }

}


module.exports = ChooseTypeRoom;