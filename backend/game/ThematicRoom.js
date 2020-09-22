import InsertTypeRoom from "./InsertTypeRoom";
import ChooseTypeRoom from "./ChooseTypeRoom";
import room from "./Room";
import Player from "./Player";
var schedule = require('node-schedule');

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function game_type(type_idx) {
    console.log("Thematic Game type: ", type_idx);
    if (type_idx == 1) return "choose";
    if (type_idx == 2) return "insert";
    if (type_idx == 3) return "drag";
}

class PastGames {
    constructor(num_of_cycles) {
        this.num_of_cycles = num_of_cycles;
        this.array = [];
    }

    static to_object(thematic_id, name, to_timestamp) {
        return {
            id : thematic_id,
            name : name,
            to_timestamp : to_timestamp
        }
    }

    push_old_date(obj) {
        if (this.num_of_cycles > this.array.length)
            this.array.push(obj);
    }

    push_new_date(obj) {
        this.array.unshift(obj);
        if (this.array.length > this.num_of_cycles) {
            this.array.pop();
        }
    }

    get_all() {
        return this.array;
    }
}


class ThematicRoom {
    constructor(query, game_conf) {
        this.query = query;
        this.GameConfig = game_conf;
        this.room_type_id = 4;
        this.game_id = -1;
        this.thematic_id = -1;
        this.thematic_name = "";
        this.game_type = "";
        this.current_game_start = new Date();
        this.current_game_end = new Date();
        this.thematic_words = [];
        this.thematic_words_dict = {};

        this.num_of_past_cycles = 4;
        this.past_curr_games = new PastGames(this.num_of_past_cycles);

        this.choose_positions = {};

        this.load_past_games();
        this.load_thematic_game();
    }

    create_object(text, id, structure_id, structure_text, position, task_position, buttons) {
        return {
            word: text,
            id: id,
            structure_id: structure_id,
            structure_text: structure_text,
            position: position,
            task_position: task_position,
            buttons: buttons
        }
    }

    load_past_games() {
        this.query.past_and_current_thematic_cycles(this.num_of_past_cycles, (error, results) => {
            if(results){
                let i;
                for(i = 0; i < results.length; i++) {
                    let row = results[i];
                    let game_info = PastGames.to_object(row.thematic_id, row.name, row.to_timestamp);
                    this.past_curr_games.push_old_date(game_info);
                }
            }
        });
    }


    async load_thematic_game() {
        this.check_for_next_game();
        await sleep(3000);
        if (this.current_game_end != null && this.current_game_end > new Date().getTime()) {
            this.thematic_words_dict = room.Room.words_to_dict(this.thematic_words);
            // console.log("thematic_words_dict", this.thematic_words_dict);
        }
    }

    end_of_thematic() {
        console.log("End of thematic!!!");
        this.load_thematic_game();
    }

    start_of_thematic() {
        console.log("Start of thematic!!!");
        this.past_curr_games.push_new_date(PastGames.to_object(this.thematic_id,
            this.thematic_name,
            this.current_game_end));
    }

    check_for_next_game() {
        this.query.next_thematic_cycle((error, result) => {
            if (error) {
                console.log(this.room_type_id, "Error occurred when querying next cycle");
                throw error;
            }
            // console.log(this.room_type_id, fields);
            // console.log(this.room_type_id, "Next cycle: ");
            let row = result[0];
            if (row == null) {
                console.log("[Thematic] there is no current game!!");
                return;
            }
            // console.log(this.room_type_id, row);
            this.game_type = game_type(row.task_type_id);
            this.thematic_name = row.name;
            this.game_id = row.id;
            this.thematic_id = row.thematic_id;
            this.current_game_start = new Date(Date.parse(row.from_timestamp));
            this.current_game_end = new Date(Date.parse(row.to_timestamp));
            console.log("Got current game start: ", this.current_game_start);
            console.log("Got current game end: ", this.current_game_end);
            if (this.current_game_start > new Date().getTime()) {
                schedule.scheduleJob(this.current_game_start, this.start_of_thematic.bind(this));
            }
            schedule.scheduleJob(this.current_game_end, this.end_of_thematic.bind(this));
            // console.log(this.room_type_id, "ncs: ", this.next_cycle_start);

            this.query.all_tasks(this.game_id, (error, words) => {
                if (error) {
                    console.log(this.room_type_id, "Error occurred when querying next words");
                    throw error;
                }
                else {
                    // console.log("words:", this.room_type_id, words);
                    this.thematic_words = [];
                    let i;
                    for (i = 0; i < words.length; i++) {
                        let word = words[i];
                        // console.log(this.room_type_id, "i: ", i);

                        if (this.game_type == "insert") {
                            this.thematic_words.push(this.create_object(word.text, word.id,
                                                word.structure_id, word.structure_text,
                                                word.csw_position, word.t_position, []));
                        } else {
                            this.query.all_possible_answers(word.id, this.game_type == "choose", (err, ans) => {
                                if (err) {
                                    console.log(this.room_type_id, "Error occurred when searching possible answers");
                                    throw err;
                                }
                                // console.log(this.room_type_id, ans, word);
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
                                            obj.choose_position = ans[j].choose_position + 1;
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
                                this.thematic_words.push(this.create_object(word.text, word.id,
                                    word.structure_id, word.structure_text,
                                    word.csw_position, word.t_position, array));
                                // console.log(this.room_type_id, "Done task: ", i, " id: ", word.id, " cycle id: ", this.next_cycle_id);
                            })
                        }
                    }
                }
            })
        });
    }

    cycle_response_json(player) {
        let is_choose = this.game_type == "choose";
        let response = {};
        response.start_of_thematic = this.current_game_start.getTime();
        response.end_of_thematic = this.current_game_end.getTime();
        response.current_time = new Date().getTime();
        response.thematic_name = this.thematic_name;
        response.number_of_rounds = this.thematic_words.length;
        response.max_select = this.GameConfig.MAX_SELECT;
        response.buttons_number = is_choose ? this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND : this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_ROUND;

        response.next_round = player.thematic_position;
        response.number_of_rounds = this.thematic_words.length;

        response.game_type = this.game_type;
        response.cycle_id =  this.game_id;
        response.words =  this.current_game_start <= new Date().getTime()  ? this.thematic_words : null;
        response.scoring = is_choose ? this.GameConfig.CHOOSE_SCORING : this.GameConfig.INSERT_SCORING;
        response.bonus_points = is_choose ? this.GameConfig.CHOOSE_BONUS : 0;

        response.past_and_current_games_info = this.past_curr_games.get_all();
        //console.log(this.room_type_id, "next game response: ")
        //console.log(this.room_type_id, response)
        return response
    }

    newPlayer_immediate(player_id, callback) {
        this.send_current_thematic(player_id, callback);
    }

    newPlayer_next_cycle(player_id, callback) {
        this.send_current_thematic(player_id, callback);
    }

    async send_current_thematic(player_id, callback) {
        if (player_id == null) {
            callback(true, null);
            return;
        }
        let player = await this.find_player(player_id);

        console.log("send_current_thematic game end:", this.current_game_end.getTime(), new Date().getTime());
        if (this.current_game_end < new Date().getTime()) {
            console.log("date: ", new Date().getTime());
            console.log("checking if new thematic: ");
            this.load_thematic_game();
            callback(false, {past_and_current_games_info : this.past_curr_games.get_all()});
        } else {
            console.log("thematic true");
            callback(false, this.cycle_response_json(player));
        }
    }

    find_player(player_id) {
        return new Promise(resolve => {
            this.query.getUser(player_id,  (error, results) => {
                if (error) {
                    console.log(this.room_type_id, "Error when query for game type id", error);
                    resolve(null);
                } else {
                    this.query.get_thematic_user(player_id, this.thematic_id, (error, thematic_user) => {
                        if (error) {
                            resolve(null);
                            console.log(this.room_type_id, "Error when get_thematic_user", error);
                            return;
                        }
                        let score = 0;
                        let position = 0;
                        if (thematic_user != null) {
                            if (thematic_user.thematic_score != null)
                                score = thematic_user.thematic_score;

                            if (thematic_user.thematic_position != null)
                                position = thematic_user.thematic_position;
                        }
                        resolve (
                            new Player(results.uid,
                                results.display_name,
                                results.experience,
                                results.choose_score,
                                results.insert_score,
                                results.drag_score,
                                results.synonym_score,
                                score,
                                position,
                                this.query)
                        );
                    });
                }
            });
        });
    }

    async set_score_for_player(player_id, cycle_id, data) {
        let player = await this.find_player(player_id);
        // console.log("set_score_for_player", player);
        if (this.thematic_words_dict == null)
            this.thematic_words_dict = room.Room.words_to_dict(this.thematic_words);

        if (cycle_id != this.game_id || player == null || this.current_game_end < new Date().getTime()) {
            return null;
        }
        if (this.game_type == "insert") {
            return InsertTypeRoom.insert_set_score(this.query, data, this.thematic_words_dict, this.GameConfig, player, this.thematic_id);
        } else if (this.game_type == "choose") {
            let reward =  ChooseTypeRoom.choose_calculate_bonus(data.words, this.GameConfig, this.choose_positions);
            console.log("reward: ", reward);
            console.log("data: ", data.words);
            // console.log("thematic_words_dict: ", this.thematic_words_dict);
            return player.update_scores(data.words, this.thematic_words_dict, reward, this.thematic_id);
        }
        return player.update_scores(data.words, this.thematic_words_dict, undefined, this.thematic_id);
    }

    post_scoreboard(cycle_id) {
        console.log("post_scoreboard on thematic !!!! WRONG");
        return null;
    }

    get_global_scoreboard(user_id, thematic_id=this.thematic_id) {
        return new Promise(resolve => {
            room.Room.get_global_scoreboard_func(user_id,
                'thematic_score',
                ((callback) => this.query.get_thematic_scoreboard(thematic_id, callback)),
                ((user_id, callback) => this.query.get_thematic_score(user_id, thematic_id, callback)),
                resolve);
        });
    }
}


module.exports = ThematicRoom;