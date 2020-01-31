import room from "./Room";


function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

class SynonymTypeRoom extends room.Room {
    constructor(query, game_conf) {
        super(query, game_conf, "synonym");

        this.number_of_rounds = this.GameConfig.SYNONYM_NUMBER_OF_WORDS_IN_ONE_CYCLE;
        this.round_pause_duration_ms = this.GameConfig.SYNONYM_ROUND_PAUSE_DURATION;
        this.buttons_number = this.GameConfig.SYNONYM_NUMBER_OF_WORDS_IN_ONE_ROUND;
        this.round_duration_ms = this.GameConfig.SYNONYM_DURATION_OF_ROUND;
        // this.collecting_results_duration_ms = this.GameConfig.DURATION_WAIT_FOR_SCORES;
        // this.show_results_duration_ms = this.GameConfig.DURATION_SHOW_SCORES;
        this.scoring = this.GameConfig.SYNONYM_SCORING;
    }

    start_game() {
    console.log(this.room_type_id, "Start cycle", this.next_cycle_start);
    super.start_game().catch((err) => { throw err });
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
            await sleep(this.GameConfig.SYNONYM_ALL_ROUNDS_DURATION - this.connect_immediate_lock_time);
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

    initialize_cycle() {
        this.query.next_cycle(this.room_type_id, (error, result) => {
            if (error) {
                console.log(this.room_type_id, "Error occurred when querying next cycle");
                throw error;
            }
            // console.log(this.room_type_id, fields);
            // console.log(this.room_type_id, "Next cycle: ");
            
            if(result.length == 0){
                return false;
            }

            let row = result[0];
            // console.log(this.room_type_id, row);
            this.next_cycle_id = row.id;
            this.current_cycle_start = this.next_cycle_start;
            this.next_cycle_start = new Date(Date.parse(row.from_timestamp));
            // console.log(this.room_type_id, "ncs: ", this.next_cycle_start);

            this.query.all_tasks(this.next_cycle_id, (error, words) => {
                if (error) {
                    console.log(this.room_type_id, "Error occurred when querying next words");
                    throw error;
                }
                else {
                    // console.log(this.room_type_id, words);
                    this.next_words = [];
                    let i;
                    for (i = 0; i < words.length; i++) {
                        let word = words[i];
                        // console.log(this.room_type_id, "i: ", i);
                        let object = {};
                        object.word = word.text;
                        object.id = word.id;
                        object.structure_id = word.structure_id;
                        object.structure_text = word.structure_text;
                        object.position = word.csw_position;
                        object.task_position = word.t_position;
                        object.buttons = [];
                        this.next_words.push(object);
                        // console.log(this.room_type_id, "Done task: ", i, " id: ", word.id, " cycle id: ", this.next_cycle_id);
                    }
                }
            })
        });
    }

    initialize_game() {
        this.initialize_cycle();
        setTimeout(this.start_game.bind(this), 1000);
    }

    static synonym_set_score(query, data, current_words_dict, GameConfig, player, thematic=false) {
        let promises = [];
        for (let i = 0; i < data.words.length; i++) {
            for (let j = 0; j < data.words[i].buttons.length; j++) {

                // console.log("TESTING:", data.words[i].buttons[j]);
                if (data.words[i].buttons[j] != '') {
                    promises.push(new Promise((resolve, reject) => {
                        // console.log("TESTING:", data.words[i].word, data.words[i].buttons[j],
                        //     current_words_dict[data.words[i].word]["structure_id"]);

                        query.find_synonym_score(data.words[i].word, data.words[i].buttons[j], 10, ((err, ans) => {

                                let score = 0;
                                if (ans.length > 0) {

                                    /*
                                    let value = ans[0].score / 100;
                                    if (value <= 0.25)
                                        score = GameConfig.SYNONYM_SCORING[0];
                                    else if (value <= 0.40)
                                        score = GameConfig.SYNONYM_SCORING[1];
                                    else if (value <= 0.55)
                                        score = GameConfig.SYNONYM_SCORING[2];
                                    else
                                        score = GameConfig.SYNONYM_SCORING[3];
                                    */
                                   score = 100 - (ans[0].score * 2);
                                } else {
                                    query.unknown_synonym_word(data.words[i].word, data.words[i].buttons[j]);
                                    score = 0;
                                }

                                if(!current_words_dict[data.words[i].word]){
                                    current_words_dict[data.words[i].word] = Array();
                                }

                                current_words_dict[data.words[i].word][data.words[i].buttons[j]] = score;

                                console.log("score: ", score);
                                //console.log("END TESTING:");
                                resolve();
                            }));
                    }))
                }
            }
        }
        return new Promise(resolve => {
            Promise.all(promises).then((values) => {
                resolve(player.update_scores(data.words, current_words_dict, null, thematic));
            })
        });
    }

    set_score_for_player(player_id, cycle_id, data) {
        let player = this.find_current_player(player_id);
        console.log(this.room_type_id, data);
        console.log(this.room_type_id, player);
        if (cycle_id != this.current_cycle_id || player == null || this.current_state == room.RoomState.SHOW_SCORES) {
            return null;
        }
        return SynonymTypeRoom.synonym_set_score(this.query, data, this.current_words_dict, this.GameConfig, player);
    }

    next_play_state() {
        super.next_play_state();
        console.log(this.room_type_id, "new game " + room.Room.date_to_string(new Date()));
        this.initialize_cycle();
    }

    get_global_scoreboard(user_id) {
        return new Promise(resolve => {
            room.Room.get_global_scoreboard_func(user_id,
                            'synonym_score',
                            this.query.get_synonym_scoreboard.bind(this.query),
                            this.query.get_synonym_score.bind(this.query),
                            resolve);
        });
    }
}


module.exports = SynonymTypeRoom;