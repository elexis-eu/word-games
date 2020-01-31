


class Player {
    constructor(player_id, display_name, experience_points, choose_score, insert_score, drag_score, synonym_score, thematic_score, thematic_position, query) {
        this.player_id = player_id;
        this.display_name = display_name;
        this.experience_points = experience_points;
        this.choose_score = choose_score;
        this.insert_score = insert_score;
        this.drag_score = drag_score;
        this.synonym_score = synonym_score;
        this.thematic_score = thematic_score;
        this.thematic_position = thematic_position;
        this.current_words_in_cycle = [];
        this.score = 0;
        this.query = query;
    }

    setScore(score) {
        this.score = score;
    }

    is_word_already_updated(word) {
        let i;
        for (i = 0; i < this.current_words_in_cycle; i++) {
            if (word === this.current_words_in_cycle[i]) {
                return true;
            }
        }
        return false;
    }
    
    update_scores(data, words_dict, bonus_points = null, thematic = false) {
        // this.score += bonus_points;
        // console.log("bonus_points: ", bonus_points);
        let last_thematic_position = this.thematic_position;

        let i;
        for (i = 0; i < data.length; i++) {
            if (data[i].word != null && !this.is_word_already_updated(data[i].word)) {
                this.current_words_in_cycle.push(data[i].word);

                if (bonus_points != null && bonus_points[data[i].word] != null) {
                    data[i].bonus_points = bonus_points[data[i].word];
                    this.score += data[i].bonus_points;
                }

                if (words_dict[data[i].word] == null) {
                    console.log("words_dict[data[i].word] is null!! ", data[i].word);
                    // console.log("words_dict[data[i].word] is null!! ", words_dict[data[i].word]);
                    // console.log(data);
                    continue;
                }

                // console.log("update_scores task_positon: ", words_dict[data[i].word]["task_position"]);
                if (thematic) {
                    this.thematic_position = Math.max(this.thematic_position, words_dict[data[i].word]["task_position"]+1);
                    if (words_dict[data[i].word]["task_position"] < last_thematic_position) {
                        console.log("Player wanted to update scores of already updated word !!!");
                        continue;
                    }
                }
                // console.log("update_scores task_positon: ", this.thematic_position);

                let task_id = words_dict[data[i].word]["id"];
                console.log("Task id: ", task_id);
                let promise_task_user = new Promise((resolve, reject) => {
                    this.query.insert_new_task_user(this.player_id, task_id, (err, res) => {
                        if (err) {
                            reject(err);
                        } else {
                            resolve(res.insertId);
                        }
                    });
                });

                let promise_words = new Promise((resolve, reject) => {
                    let words = [];
                    let j;
                    data[i].scores = [];
                    for (j = 0; j < data[i].buttons.length; j++) {
                        let button_word = data[i].buttons[j];
                        if (typeof button_word !== "string")
                            button_word = data[i].buttons[j].word;

                        words.push({
                            "word": button_word,
                            "tpa_id": words_dict[data[i].word][button_word+"_tpa_id"]
                        });
                        let scr = words_dict[data[i].word][button_word];
                        if (scr !== undefined) {
                            // console.log("Player " + this.player_id + " scored: " + scr + " for word: " + button_word + " " + data[i].word);
                            data[i].scores.push({
                                "word": button_word,
                                "score": scr
                            });
                            // console.log("score: ", scr);
                            this.score += scr
                        }
                        else {
                            console.log("Error: Player " + this.player_id + " scored: " + scr + " for word: " + button_word + " " + data[i].word);
                            data[i].scores.push({
                                "word": button_word,
                                "score": 0
                            });
                        }
                    }
                    resolve(words);
                });

                Promise.all([
                        promise_task_user.catch(error => { console.log("Player.js promise task user failed !! ", error); }),
                        promise_words.catch(error => { console.log("Player.js promise words failed !! ", error); })
                ]).then((values) => {
                    console.log("Promise all: ", values);
                    let task_user_id = values[0];
                    let words = values[1];
                    words.forEach(word_obj => {
                        let word = word_obj.word;
                        let tpa_id = word_obj.tpa_id;
                        // console.log("tpa id: ", word);
                        // console.log("tpa id: ", tpa_id);
                        this.query.insert_task_possible_answer_procedure(task_id, task_user_id, word, tpa_id)
                    });
                });
            }
        }
        console.log("Player " + this.player_id + " scored " + this.score);
        if (thematic) {
            this.new_cycle("thematic", thematic);
        }
        return data;
    }

    new_cycle(game_type, thematic=null) {
        if (game_type == "choose") {
            this.choose_score += this.score;
            this.query.update_choose_score(this.player_id, this.score);
        } else if (game_type == "insert") {
            this.insert_score += this.score;
            this.query.update_insert_score(this.player_id, this.score);
        } else if (game_type == "drag") {
            this.drag_score += this.score;
            this.query.update_drag_score(this.player_id, this.score);
        } else if (game_type == "synonym") {
            this.synonym_score += this.score;
            this.query.update_synonym_score(this.player_id, this.score);
        } else if (game_type == "thematic") {
            this.thematic_score += this.score;
            this.query.set_thematic_score(this.player_id, thematic, this.thematic_score, this.thematic_position);
        }
        this.experience_points += this.score;
        this.score = 0;
        this.current_words_in_cycle = []
    }
}

module.exports = Player;