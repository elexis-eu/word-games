import Player from "./Player";
import '@babel/polyfill'

// const GameConfig = {
//     "MAX_AVAILABLE_PLAYERS" : 9999999999, // TODO: change it
//     "CHOOSE_DURATION_OF_ROUND" : 5000,  // in milliseconds -> gameplay duration
//     "ROUND_PAUSE_DURATION" : 1000,
//     "DURATION_WAIT_FOR_SCORES" : 10000, // in milliseconds
//     "DURATION_SHOW_SCORES" : 10000, // in milliseconds
//     "MAX_SELECT" : 3,
//     "NUMBER_OF_WORDS_IN_ONE_CYCLE" : 5, // number of cycles
//     "NUMBER_OF_WORDS_IN_ONE_ROUND" : 9, // how many buttons will be in one cycle
//     "CHOOSE_ALL_ROUNDS_DURATION" : (GameConfig.CHOOSE_DURATION_OF_ROUND + GameConfig.ROUND_PAUSE_DURATION) * GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE,
//     "CHOOSE_DURATION_WHOLE_CYCLE" : GameConfig.CHOOSE_ALL_ROUNDS_DURATION + GameConfig.DURATION_WAIT_FOR_SCORES + GameConfig.DURATION_SHOW_SCORES,
//
//     "DRAG_ROUND_PAUSE_DURATION" : 5000,
//     "DRAG_DURATION_OF_ROUND" : 15000,  // in milliseconds
//     "DRAG_DURATION_WHOLE_CYCLE" : GameConfig.DRAG_DURATION_OF_ROUND + GameConfig.DRAG_ROUND_PAUSE_DURATION + GameConfig.DURATION_WAIT_FOR_SCORES + GameConfig.DURATION_SHOW_SCORES,
// };

const RoomState = {
    "UNDEFINED" : 0,
    "IN_GAME" : 1,
    "WAITING_FOR_SCORES" : 2,
    "SHOW_SCORES" : 3,
};

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}


class Room {

    constructor(query, game_conf, game_type) {
        this.game_type = game_type;
        this.room_type_id = -1;
        this.current_cycle_id = -1;
        this.next_cycle_id = -1;
        this.query = query;
        this.GameConfig = game_conf;
        this.GameConfig.CHOOSE_ONE_ROUND_DURATION = (this.GameConfig.CHOOSE_DURATION_OF_ROUND + this.GameConfig.ROUND_PAUSE_DURATION);
        this.GameConfig.CHOOSE_ALL_ROUNDS_DURATION = this.GameConfig.CHOOSE_ONE_ROUND_DURATION * this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE - this.GameConfig.ROUND_PAUSE_DURATION;
        this.GameConfig.CHOOSE_DURATION_WHOLE_CYCLE = this.GameConfig.CHOOSE_ALL_ROUNDS_DURATION + this.GameConfig.DURATION_WAIT_FOR_SCORES + this.GameConfig.DURATION_SHOW_SCORES;

        this.GameConfig.INSERT_ONE_ROUND_DURATION = (this.GameConfig.INSERT_DURATION_OF_ROUND + this.GameConfig.INSERT_ROUND_PAUSE_DURATION);
        this.GameConfig.INSERT_ALL_ROUNDS_DURATION = this.GameConfig.INSERT_ONE_ROUND_DURATION * this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE - this.GameConfig.INSERT_ROUND_PAUSE_DURATION;
        this.GameConfig.INSERT_DURATION_WHOLE_CYCLE = this.GameConfig.INSERT_ALL_ROUNDS_DURATION + this.GameConfig.DURATION_WAIT_FOR_SCORES + this.GameConfig.DURATION_SHOW_SCORES;

        this.GameConfig.SYNONYM_ONE_ROUND_DURATION = (this.GameConfig.SYNONYM_DURATION_OF_ROUND + this.GameConfig.SYNONYM_ROUND_PAUSE_DURATION);
        this.GameConfig.SYNONYM_ALL_ROUNDS_DURATION = this.GameConfig.SYNONYM_ONE_ROUND_DURATION * this.GameConfig.SYNONYM_NUMBER_OF_WORDS_IN_ONE_CYCLE - this.GameConfig.SYNONYM_ROUND_PAUSE_DURATION;
        this.GameConfig.SYNONYM_DURATION_WHOLE_CYCLE = this.GameConfig.SYNONYM_ALL_ROUNDS_DURATION + this.GameConfig.DURATION_WAIT_FOR_SCORES + this.GameConfig.DURATION_SHOW_SCORES;

        this.GameConfig.DRAG_ONE_ROUND_DURATION = this.GameConfig.DRAG_DURATION_OF_ROUND / this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND;
        this.GameConfig.DRAG_ROUNDS_DURATION = this.GameConfig.DRAG_DURATION_OF_ROUND;
        this.GameConfig.DRAG_DURATION_WHOLE_CYCLE = this.GameConfig.DRAG_ROUNDS_DURATION + this.GameConfig.DURATION_WAIT_FOR_SCORES + this.GameConfig.DURATION_SHOW_SCORES;
        // console.log(this.room_type_id, "Config: ", this.GameConfig);
        this.current_state = RoomState.UNDEFINED;
        this.lock_immediate = false;

        this.number_of_rounds = this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE;
        this.max_select = this.GameConfig.MAX_SELECT;
        this.round_pause_duration_ms = this.GameConfig.ROUND_PAUSE_DURATION;
        this.buttons_number = this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND;
        this.round_duration_ms = this.GameConfig.CHOOSE_DURATION_OF_ROUND;
        this.collecting_results_duration_ms = this.GameConfig.DURATION_WAIT_FOR_SCORES;
        this.show_results_duration_ms = this.GameConfig.DURATION_SHOW_SCORES;
        this.connect_immediate_lock_time = this.GameConfig.CONNECT_IMMEDIATE_LOCK_TIME;

        this.playerNum = 0;
        this.current_players = [];
        this.next_players = [];

        this.current_words = {};
        this.next_words = {};
        this.current_cycle_start = new Date();
        this.next_cycle_start = new Date();
        this.scoreboard = {};

        this.choose_positions = {};

        this.scoring = [];
        this.bonus = 0;

        query.game_type_id(game_type, (error, results) => {
            if (error) {
                console.log(this.room_type_id, "Error when query for game type id");
            } else {
                console.log(this.room_type_id, "Game type id for Choose");
                // console.log(results);
                // console.log(results[0].id);
                this.room_type_id = results[0].id;
                console.log("Starting game", this.room_type_id);
                this.initialize_game();
            }
        });
    }

    initialize_cycle() {
        this.query.next_cycle(this.room_type_id, (error, result) => {
            if (error) {
                console.log(this.room_type_id, "Error occurred when querying next cycle");
                throw error;
            }
            // console.log(this.room_type_id, fields);
            // console.log(this.room_type_id, "Next cycle: ");
            console.log("NEXT CYCLE");
            console.log(result);

            if(result.length == 0){
                return false;
            }

            let row = result[0];
            // console.log(this.room_type_id, row);
            this.next_cycle_id = row.id;
            this.current_cycle_start = this.next_cycle_start;
            this.next_cycle_start = new Date(Date.parse(row.from_timestamp));
            // console.log(this.room_type_id, " ncs: ", this.next_cycle_start.getTime(), " sql send me: ", row.from_timestamp);

            this.query.all_tasks(this.next_cycle_id, (error, words) => {
                if (error) {
                    console.log(this.room_type_id, "Error occurred when querying next words");
                    throw error;
                }
                else {
                    // console.log("words:", this.room_type_id, words);
                    this.next_words = [];
                    let i;
                    for (i = 0; i < words.length; i++) {
                        let word = words[i];
                        // console.log(this.room_type_id, "i: ", i);

                        this.query.all_possible_answers(word.id, this.game_type == "choose", (err, ans) => {
                            if (err) {
                                console.log(this.room_type_id, "Error occurred when searching possible answers");
                                throw err;
                            }
                            // console.log(this.room_type_id, ans, word);
                            let object = {};
                            let array = [];
                            let j;
                            for (j = 0; j < ans.length; j++) {
                                if (this.game_type == "choose") {
                                    let obj = {
                                        "tpa_id": ans[j].id,
                                        "word": ans[j].text,
                                        "score": ans[j].score,
                                        "group": ans[j].group_position
                                    };
                                    if (ans[j].choose_position != null) {
                                        obj.choose_position = ans[j].choose_position+1;
                                        if (this.choose_positions[word.text] == null) {
                                            this.choose_positions[word.text] = {};
                                        }
                                        this.choose_positions[word.text][ans[j].text] = ans[j].choose_position;
                                    }
                                    array.push(obj);
                                } else {
                                    array.push({
                                        "tpa_id": ans[j].id,
                                        "word": ans[j].text,
                                        "score": ans[j].score
                                    });
                                }
                            }
                            object.word = word.text;
                            object.id = word.id;
                            object.structure_text = word.structure_text;
                            object.position = word.csw_position;
                            object.task_position = word.t_position;
                            object.buttons = array;
                            this.next_words.push(object);
                            // console.log(this.room_type_id, "Done task: ", i, " id: ", word.id, " cycle id: ", this.next_cycle_id);
                        })
                    }
                }
            })
        });
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
            await sleep(this.GameConfig.CHOOSE_ALL_ROUNDS_DURATION - this.connect_immediate_lock_time);
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
  
    next_play_state() {
        console.log(this.room_type_id, "Room new game " + Room.date_to_string(new Date()));
        this.current_state = RoomState.IN_GAME;
        this.scoreboard = {};

        // this.next_cycle_start = new Date().getTime() + this.GameConfig.CHOOSE_DURATION_WHOLE_CYCLE; // TODO: SPREMENI

        this.current_players = this.next_players;
        this.next_players = [];

        this.current_words = this.next_words;
        this.current_words_dict = Room.words_to_dict(this.current_words);
        //print_words_obj(this.current_words, this.game_type == "choose");
        // this.thematic_words = this.query.getWords(this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE, this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND);

        this.current_cycle_id = this.next_cycle_id;
        // this.next_cycle_id = Room.unique_game_id(); // TODO: SPREMENI
    }
  
    wait_for_scores_state() {
        this.current_state = RoomState.WAITING_FOR_SCORES;
        console.log(this.room_type_id, "wait_for_scores " + Room.date_to_string(new Date()));
        
    }

    show_scores_state() {
        console.log(this.room_type_id, "show_scores " + Room.date_to_string(new Date()));
        this.build_scoreboard();
        this.current_state = RoomState.SHOW_SCORES;
        console.log(this.room_type_id, "show_scores " + Room.date_to_string(new Date()));
        this.current_players.forEach(player => player.new_cycle(this.game_type));
    }

    static print_words_obj(words, print_group=false) {
        console.log("Words:");
        let i;
        for (i = 0; i < words.length; i++) {
            console.log("word: " + words[i].word);
            console.log("word_position: " + words[i].position);
            console.log("task_position: " + words[i].task_position);
            console.log("buttons: ");
            let j = 0;
            if (words[i].buttons != null) {
                for (j = 0; j < words[i].buttons.length; j++) {
                    console.log("    word: " + words[i].buttons[j].word);
                    console.log("    score: " + words[i].buttons[j].score);
                    if (print_group) {
                        console.log("   group: " + words[i].buttons[j].group)
                    }
                }
            }
        }
    };

    static date_to_string(date) {
        return date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + "." + date.getMilliseconds();
    }

    build_scoreboard() {
        this.current_players.sort(Room.compare_player);
        let board = [];
        let i;
        let current_score = -1;
        let previous_position = -1;
        for (i = 0; i < this.current_players.length; i++) {
            let position = i+1;
            if (current_score == this.current_players[i].score)
                position = previous_position;
            previous_position = position;
            current_score = this.current_players[i].score;

            board.push({
                "display_name" : this.current_players[i].display_name,
                "score" : this.current_players[i].score,
                "position" : position
            })
        }
        this.scoreboard = board;
        console.log(this.room_type_id, "Scoreboard: ");
        console.log(this.room_type_id, this.scoreboard)
    }

    static compare_player(player1, player2) {
        if (player1.score < player2.score) 
            return 1;
        if (player1.score > player2.score) 
            return -1;
        return 0;
    }

    set_score_for_player(player_id, cycle_id, data) {
        let player = this.find_current_player(player_id);
        // console.log(this.room_type_id, data);
        // console.log(this.room_type_id, player);
        if (cycle_id != this.current_cycle_id || player == null || this.current_state == RoomState.SHOW_SCORES) {
            // console.log(this.room_type_id, "Set score failed: Cycle id:");
            // console.log(this.room_type_id, cycle_id);
            // console.log(this.room_type_id, this.current_cycle_id);
            // console.log(this.room_type_id, cycle_id != this.current_cycle_id);
            // console.log(this.room_type_id, cycle_id !== this.current_cycle_id);
            // console.log(this.room_type_id, "player:");
            // console.log(this.room_type_id, player);
            // console.log(this.room_type_id, "Curr state: ");
            // console.log(this.room_type_id, this.current_state);
            // console.log(this.room_type_id, RoomState.WAITING_FOR_SCORES);
            // console.log(this.room_type_id, this.current_state == RoomState.SHOW_SCORES);
            // console.log(this.room_type_id, this.current_state === RoomState.SHOW_SCORES);
            return null;
        }
        // console.log(this.room_type_id, "update scores");
        // print_words_obj(data.words);
        // print_words_obj(this.current_words);
        // console.log(this.room_type_id, "Endlsfj");
        return player.update_scores(data.words, this.current_words_dict);
    }

    post_scoreboard(cycle_id) {
        // console.log(this.room_type_id, "state:");
        // console.log(this.room_type_id, this.current_state);
        // console.log(this.room_type_id, RoomState.SHOW_SCORES);
        if (cycle_id != this.current_cycle_id) {
            // console.log("WRONG CYCLE ID");
            // console.log(this.room_type_id, "cycle:");
            // console.log(this.room_type_id, cycle_id);
            // console.log(this.room_type_id, ":");
            // console.log(this.room_type_id, this.current_cycle_id);
            return null;
        }
        if (this.current_state == RoomState.SHOW_SCORES) {
            console.log(this.room_type_id, "scoreboard retured: ");
            console.log(this.room_type_id, this.scoreboard);
            return this.scoreboard
        }
        console.log(this.room_type_id, "Error: there is no scoreboard !!!!!");
        return null
    }

    static words_to_dict(words) {
        let dict = {};
        let i;
        for (i = 0; i < words.length; i++) {
            dict[words[i].word] = {};
            dict[words[i].word]["id"] = words[i].id;
            if (words[i].structure_id != null)
                dict[words[i].word]["structure_id"] = words[i].structure_id;
            if (words[i].task_position != null)
                dict[words[i].word]["task_position"] = words[i].task_position;

            // console.log("Debug id: ", dict[words[i].word]["id"]);
            let j = 0;
            if (words[i].buttons != null) {
                for (j = 0; j < words[i].buttons.length; j++) {
                    dict[words[i].word][words[i].buttons[j].word] = words[i].buttons[j].score;
                    dict[words[i].word][words[i].buttons[j].word+"_tpa_id"] = words[i].buttons[j].tpa_id;
                }
            }
        }
        //console.log(this.room_type_id, dict)
        return dict
    }
  
    // next_cycle_response_json() {
    //     return this.cycle_response_json(false)
    // }
    //
    // current_cycle_response_json() {
    //     if (this.current_state == RoomState.IN_GAME && !this.lock_immediate){
    //         return this.cycle_response_json(true);
    //     }
    //     return this.cycle_response_json(false);
    // }

    cycle_response_json(isImmediate) {
        let response = {};
        response.game_start = isImmediate ? this.current_cycle_start.getTime() : this.next_cycle_start.getTime();
        response.current_time = new Date().getTime();
        response.number_of_rounds = this.number_of_rounds;
        response.max_select = this.max_select;
        response.round_pause_duration_ms = this.round_pause_duration_ms;
        response.buttons_number = this.buttons_number;
        response.round_duration_ms = this.round_duration_ms;
        response.collecting_results_duration_ms = this.collecting_results_duration_ms;
        response.connect_immediate_lock_time = this.connect_immediate_lock_time;
        response.show_results_duration_ms = this.show_results_duration_ms;
        response.cycle_id =  isImmediate ? this.current_cycle_id : this.next_cycle_id;
        response.words =  isImmediate ? this.current_words :  this.next_words;
        response.scoring = this.scoring;
        response.bonus_points = this.bonus;
        //console.log(this.room_type_id, "next game response: ")
        //console.log(this.room_type_id, response)
        return response
    }

    find_current_player(player_id) {
        let i;
        for (i = 0; i < this.current_players.length; i++) {
            if (this.current_players[i].player_id == player_id) {
                return this.current_players[i];
            }
        }
        return null
    }

    find_next_player(player_id) {
        let i;
        for (i = 0; i < this.next_players.length; i++) {
            if (this.next_players[i].player_id == player_id) {
                return this.next_players[i];
            }
        }
        return null
    }

    player_already_exists(player_id) {
       return this.find_current_player(player_id) != null
    }

    newPlayer_immediate(player_id, callback) {
        this.newPlayer(player_id, true, callback);
    }

    newPlayer_next_cycle(player_id, callback) {
        this.newPlayer(player_id, false, callback);
    }
  
    newPlayer(player_id, isImmediate, callback) {
        if (this.playerNum >= this.GameConfig.MAX_AVAILABLE_PLAYERS) {
            return "ni več placa zate";
        }
        // check if is current game state in game and it is not last round
        isImmediate = isImmediate && this.current_state == RoomState.IN_GAME && !this.lock_immediate;

        let response_json = this.cycle_response_json(isImmediate);
        /*if (this.player_already_exists(player_id)) {
            return "Igralec s tem vzdevkom je že v igri"
        }*/

        let player = this.find_current_player(player_id);
        if (player == null) {
            player = this.find_next_player(player_id);
            if (player == null) {
                this.query.getUser(player_id,  (error, results) => {
                    if (error) {
                        console.log(this.room_type_id, "Error when query for user id");
                        callback(error, null);
                    } else {
                        if (player_id == null)
                            response_json.player_id = results.uid;
                        player = new Player(results.uid,    results.display_name,
                                                            results.experience,
                                                            results.choose_score,
                                                            results.insert_score,
                                                            results.drag_score,
                                                            results.synonym_score,
                                                            null,
                                                            null,
                                                            this.query);
                        if (isImmediate) {
                            console.log("New player added!");
                            this.current_players.push(player);
                        }
                        else {
                            this.next_players.push(player);
                        }
                        console.log(this.room_type_id, "New player: ", player.display_name);
                        callback(false, response_json);
                    }
                });
                return;
            }
            else {
                console.log(this.room_type_id, "Existing player wants to connect twice...");
                callback(false, response_json);
                return;
            }
        }
        else if (!isImmediate && this.find_next_player(player_id) == null) {
            // console.log(this.room_type_id, "Player is going to play another cycle: ", player);
            this.next_players.push(player);
        }
        else {
            console.log(this.room_type_id, "Existing player: ", player.display_name);
        }
        // this.playerNum++;
        callback(false, response_json);
    }

    static get_global_scoreboard_func(user_id, score_type, global_scoreboard_func, global_score_func, resolve) {
        global_scoreboard_func((err, ans) => {
            // console.log(ans);
            let scoreboard = [];
            let i = 0;
            if (ans != null) {
                for (i = 0; i < ans.length; i++) {
                    scoreboard.push({
                        "display_name": ans[i].display_name,
                        "score": ans[i][score_type]
                    })
                }
            }
            global_score_func(user_id, (er, answer) => {
                if (er){
                    return console.log("get score failed !!! ", er.message);
                }
                let display_name = null;
                let user_score = null;
                let position = null;
                if (answer != null && answer[0] != null) {
                    user_score = answer[0][score_type];
                    position = answer[0].position;
                    display_name = answer[0].display_name;
                }
                resolve({
                    "user_score" : {
                        "display_name": display_name,
                        "score" : user_score,
                        "position" : position,
                    },
                    "scoreboard" : scoreboard
                });
            });
        });
    }
}

module.exports = {
    RoomState   : RoomState,
    Room        : Room
};
