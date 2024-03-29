import Query from "./Query";
import '@babel/polyfill'
import { createDecipher, createHash } from "crypto";

class Collocations {

    constructor(query, game_config) {
        this.query = query;
        this.GameConfig = game_config;
    }

    GetDifficultyForLevel(levelID){
        return levelID;
    }

    GetChooseLimit(){
        return 9;
    }

    GetGameTypeID(game_type){
        let game_type_list = {"choose": 1, "insert": 2, "drag": 3}
        return game_type_list[game_type];
    }

    shuffle(array) {
        var currentIndex = array.length, temporaryValue, randomIndex;
      
        // While there remain elements to shuffle...
        while (0 !== currentIndex) {
      
          // Pick a remaining element...
          randomIndex = Math.floor(Math.random() * currentIndex);
          currentIndex -= 1;
      
          // And swap it with the current element.
          temporaryValue = array[currentIndex];
          array[currentIndex] = array[randomIndex];
          array[randomIndex] = temporaryValue;
        }
      
        return array;
    }

    async GetLevelInfo(levelID, user, type){
        try{

            let rooms = await this.query.get_user_collocations_level(user, levelID, type);

            if(rooms.length == 0){
                let new_rooms = await this.query.get_defined_collocation_level(levelID);

                console.log(new_rooms);

                let position_counter = 1;
                for (var i = 0; i < new_rooms.length; i++) {
                    var row = new_rooms[i];
                    let inserted_room = await this.query.insert_collocation_user_level(user, type, row.id_collocation_level, position_counter);
                    position_counter++;
                }

                rooms = false;
                rooms = await this.query.get_user_collocations_level(user, levelID, type);

                console.log("Rooms:");
                console.log(rooms);

                return {"rooms": rooms};
            }

            let user_score = 0;
            let user_score_arr = await this.query.get_user_collocations_score_level(user, levelID, type);

            if(user_score_arr.length == 0){
                user_score = user_score_arr[0].sum_score;
            }
            
            console.log("Rooms:");
            console.log(rooms);

            return {"rooms": rooms, "score": user_score};

        } catch(e){
            throw e;
        }
        
    }

    async GetGameInfo(col_level_ID, user, type){
        try{
            
            let response = {};

            let collocation_level_info = await this.query.get_defined_collocation_byid(col_level_ID);

            console.log(collocation_level_info);

            if(collocation_level_info.length == 1){

                if(collocation_level_info[0].game_type_name == 'choose'){

                    let chooseButtons = await this.query.get_choose_words(collocation_level_info[0].id, collocation_level_info[0].headword1, this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND);

                    let position_counter = 1;
                    let response_buttons = [];
                    let choose_scores = this.GameConfig.SOLO_CHOOSE_SCORING;

                    for (var i = 0; i < chooseButtons.length; i++) {
                        var row = chooseButtons[i];

                        let word_object =   {   "choose_position": position_counter, 
                                                "group" : ((position_counter-1)%this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE)+1, 
                                                "score": choose_scores[Math.floor((position_counter-1)/this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE)] * collocation_level_info[0].points_multiplier,
                                                "word": row.text,
                                                "collocation_id": row.collocation_id
                                            };

                        response_buttons.push(word_object);

                        position_counter++;
                    }

                    response_buttons = this.shuffle(response_buttons);

                    let words = Array({ "word": collocation_level_info[0].headword1, 
                                        "position": collocation_level_info[0].headword_position, 
                                        "structure_text": collocation_level_info[0].text, 
                                        "buttons" : response_buttons
                                    });

                    response = {    "words": words,   
                                        "max_round_score": 300 * collocation_level_info[0].points_multiplier,
                                        "next_round": 1,
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.MultiplyScoreValues(this.GameConfig.SOLO_CHOOSE_SCORING, collocation_level_info[0].points_multiplier),
                                        "bonus_points": this.GameConfig.SOLO_CHOOSE_BONUS * collocation_level_info[0].points_multiplier
                                    };

                    
                    let hash = createHash('md5').update(JSON.stringify(response) + user + type + Date()).digest('hex');
                    response["log_session"] = hash;

                    response_buttons.forEach(
                        async function(item){
                            await this.query.collocation_save_user_choose(collocation_level_info[0].id_collocation_level, user, type, item['choose_position'], item['group'], item['score'], item['word'], item['collocation_id'], hash);
                        }.bind(this)
                    );

                } else if(collocation_level_info[0].game_type_name == 'insert'){

                    let words = Array({ "word": collocation_level_info[0].headword1, 
                                        "position": collocation_level_info[0].headword_position, 
                                        "structure_text": collocation_level_info[0].text,
                                        "structure_id": collocation_level_info[0].id,
                                        "buttons" : []
                                    });

                    response = {        "words": words,   
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.SOLO_INSERT_MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.SOLO_INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.SOLO_INSERT_NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.MultiplyScoreValues(this.GameConfig.SOLO_INSERT_SCORING, collocation_level_info[0].points_multiplier),
                                        "max_round_score": (this.GameConfig.SOLO_INSERT_SCORING[0] * 3 + this.GameConfig.SOLO_INSERT_BONUS_TOP_POSITION * this.GameConfig.SOLO_INSERT_BONUS_TOP_POSITION_POINTS) * collocation_level_info[0].points_multiplier,
                                        "bonus_points": 0
                                    };
                    
                    let hash = createHash('md5').update(JSON.stringify(response) + user + type + Date()).digest('hex');
                    response["log_session"] = hash;
                

                } else if(collocation_level_info[0].game_type_name == 'drag'){

                    let dragWords = await this.query.get_drag_words_single(collocation_level_info[0].id, collocation_level_info[0].headword1, this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND_FIRST);
                    let dragWords2 = await this.query.get_drag_words_single(collocation_level_info[0].id, collocation_level_info[0].headword2, this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND_SECOND);

                    dragWords = dragWords.concat(dragWords2);

                    let position_counter = 1;

                    let words = Array();

                    let random_word_position = Math.floor(Math.random() * Math.floor(dragWords.length)) + 1;

                    let last_word_position = 1;

                    console.log("Random position:"+random_word_position);

                    for (var i = 0; i < dragWords.length; i++) {
                        var row = dragWords[i];

                        if(i == random_word_position){
                            //continue;
                        }

                        let response_buttons = [];

                        if(row.nr_distinct == 2){

                            let word_object1 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[0] * collocation_level_info[0].points_multiplier, 
                                "word": collocation_level_info[0].headword1,
                                "collocation_id": row.collocation_id
                            };

                            response_buttons.push(word_object1);

                            let word_object2 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[0] * collocation_level_info[0].points_multiplier, 
                                "word": collocation_level_info[0].headword2,
                                "collocation_id": row.collocation_id
                            };

                            response_buttons.push(word_object2);

                            let word_object3 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[2] * collocation_level_info[0].points_multiplier, 
                                "word": "",
                                "collocation_id": 0
                            };

                            response_buttons.push(word_object3);

                        } else {

                            if(row.headword == collocation_level_info[0].headword1){

                                let word_object1 =   {   
                                    "score": this.GameConfig.SOLO_DRAG_SCORING[0] * collocation_level_info[0].points_multiplier, 
                                    "word": collocation_level_info[0].headword1,
                                    "collocation_id": row.collocation_id
                                };
    
                                response_buttons.push(word_object1);

                                let word_object2 =   {   
                                    "score": this.GameConfig.SOLO_DRAG_SCORING[2] * collocation_level_info[0].points_multiplier, 
                                    "word": collocation_level_info[0].headword2,
                                    "collocation_id": 0
                                };
    
                                response_buttons.push(word_object2);
                            }

                            if(row.headword == collocation_level_info[0].headword2){

                                let word_object1 =   {   
                                    "score": this.GameConfig.SOLO_DRAG_SCORING[0] * collocation_level_info[0].points_multiplier, 
                                    "word": collocation_level_info[0].headword2,
                                    "collocation_id": row.collocation_id
                                };
    
                                response_buttons.push(word_object1);

                                let word_object2 =   {   
                                    "score": this.GameConfig.SOLO_DRAG_SCORING[2] * collocation_level_info[0].points_multiplier, 
                                    "word": collocation_level_info[0].headword1,
                                    "collocation_id": 0
                                };
    
                                response_buttons.push(word_object2);
                            }

                            let word_object3 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[1] * collocation_level_info[0].points_multiplier, 
                                "word": "",
                                "collocation_id": 0
                            };

                            response_buttons.push(word_object3);

                        }

                        words.push({    "word": row.text, 
                                        "position": row.position, 
                                        "task_position": position_counter, 
                                        "structure_text": collocation_level_info[0].text,
                                        "structure_id": collocation_level_info[0].id,
                                        "buttons" : response_buttons
                        });

                        last_word_position = row.position;

                        position_counter++;
                    }

                    if((position_counter-1) < this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND){
                        //add unknown words

                        //let randomWords = await this.query.get_random_words(this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND - (position_counter-1));

                        let randomWords = await this.query.get_random_words_structure(collocation_level_info[0].structure_id, this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND - (position_counter-1), collocation_level_info[0].headword1, collocation_level_info[0].headword2);

                        for (var i = 0; i < randomWords.length; i++) {
                            var row = randomWords[i];
                            
                            let response_buttons = [];

                            let word_object1 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[2] * collocation_level_info[0].points_multiplier, 
                                "word": collocation_level_info[0].headword1,
                                "collocation_id": 0
                            };

                            response_buttons.push(word_object1);

                            let word_object2 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[2] * collocation_level_info[0].points_multiplier, 
                                "word": collocation_level_info[0].headword2,
                                "collocation_id": 0
                            };

                            response_buttons.push(word_object2);

                            let word_object3 =   {   
                                "score": this.GameConfig.SOLO_DRAG_SCORING[0] * collocation_level_info[0].points_multiplier, 
                                "word": "",
                                "collocation_id": 0
                            };

                            response_buttons.push(word_object3);

                            words.push({    "word": row.text, 
                                            "position": last_word_position,
                                            "task_position": position_counter, 
                                            "structure_text": collocation_level_info[0].text,
                                            "structure_id": collocation_level_info[0].id,
                                            "buttons" : response_buttons
                                        });

                            position_counter++;

                        }
                    }

                    words = this.shuffle(words);

                    response = {        "words": words,   
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.SOLO_INSERT_MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.MultiplyScoreValues(this.GameConfig.DRAG_SCORING, collocation_level_info[0].points_multiplier),
                                        "bonus_points": 0,
                                        "bonus_condition": this.GameConfig.SOLO_DRAG_BONUS_CONDITION,
                                        "bonus_condition_points": this.GameConfig.SOLO_DRAG_BONUS_POINTS,
                                        "double_points_round": (Math.floor(Math.random() * Math.floor(this.GameConfig.SOLO_DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND) + 1))
                                    };

                    let hash = createHash('md5').update(JSON.stringify(response) + user + type + Date()).digest('hex');
                    response["log_session"] = hash;

                } else {
                    throw new Error("Unknown game type!");
                }

            } else {
                throw new Error("Missing collocation level!");
            }

            
            console.log("Response:");
            console.log(response);

            return response;

        } catch(e){
            let error_response = {"error" : true, "error_msg": e.message};
            
            return error_response;
        }
        
    }

    async IncreaseCollocationWeight(collocation_id, game_type){
        try{

            let game_type_id = this.GetGameTypeID(game_type);

            let priority = await this.query.get_collocation_priority(collocation_id, game_type_id);

            if(priority.length == 0){

                let create_priority = await this.query.create_collocation_priority(collocation_id, game_type_id, 0, this.GameConfig.SPECIFIC_WEIGHT, 0, this.GameConfig.WEIGHT_LIMIT);

                priority = await this.query.get_collocation_priority(collocation_id, game_type_id);

                /*
                console.log(new_words);

                let position_counter = 1;
                for (var i = 0; i < new_words.length; i++) {
                    var row = new_words[i];
                    let inserted_word = await this.query.insert_collocation_user_level(user, type, row.id_collocation_level, position_counter);
                    position_counter++;
                }

                words = false;
                words = await this.query.get_user_collocations_level(user, levelID, type);

                console.log("Words:");
                console.log(words);

                return words;
                */
            }
            
            if(priority.length == 1) {
                let row = priority[0];
                
                let priority_value = row.priority;
                let total_weight = row.total_weight + row.specific_weight;
                
                if(total_weight > row.weight_limit){
                    total_weight = 0;
                    priority_value++;
                }

                let updated_priority = await this.query.update_collocation_priority(collocation_id, game_type_id, priority_value, total_weight);
            }
            
            console.log("Priority:");
            console.log(priority);

            return {"message": "OK"};

        } catch(e){
            throw e;
        }
        
    }

    async LogDrag(level_id, uid, body){
        try{

            let collocation_level_info = await this.query.get_defined_collocation_byid(level_id);

            await this.query.collocation_log_drag(collocation_level_info[0].id_collocation_level, collocation_level_info[0].level, collocation_level_info[0].structure_id, body.word_shown, body.word_selected, body.collocation_id, body.score, uid);

            //console.log(body.word_shown);
            //console.log(body.word_selected);

            return {"message": "OK"};

        } catch(e){
            throw e;
        }
        
    }

    async LogChoose(level_id, uid, body){
        try{

            let collocation_level_info = await this.query.get_defined_collocation_byid(level_id);

            await this.query.collocation_log_choose(collocation_level_info[0].id_collocation_level, collocation_level_info[0].level, collocation_level_info[0].structure_id, collocation_level_info[0].headword1, body.word_selected, body.collocation_id, body.score, uid, body.log_session);

            //console.log(body.word_shown);
            //console.log(body.word_selected);

            return {"message": "OK"};

        } catch(e){
            throw e;
        }
        
    }

    async GetCampaignLevel(user){
        try{
            let maxLevel = 1;
            let scoreLevelSet = await this.query.get_collocation_campaign_level_max(user);

            if(scoreLevelSet.length == 1){
                if(scoreLevelSet[0].max_level){
                    maxLevel = scoreLevelSet[0].max_level;
                } else {
                    maxLevel = 1;
                }
            }

            return {campaignLevel: maxLevel};

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    async CheckInputWords(levelID, user, type, body){
        try{

            //console.log(body);

            let score1 = 0;
            let score2 = 0;
            let score3 = 0;

            let col1 = 0;
            let col2 = 0;
            let col3 = 0;

            let variant1 = 0;
            let variant2 = 0;
            let variant3 = 0;

            let words = await this.query.get_collocation_words(body.collocationLevelID);

            let collocation_level_info = await this.query.get_defined_collocation_byid(body.collocationLevelID);
            
            if(words.length != 0){

                for (var i = 0; i < words.length; i++) {
                    let row = words[i];

                    //console.log(row);

                    if(row.text == body.word1Text.toLowerCase()){
                        score1 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                        col1 = row.id;
                    } else if(row.text == body.word2Text.toLowerCase()){
                        score2 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                        col2 = row.id;
                    } else if(row.text == body.word3Text.toLowerCase()){
                        score3 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                        col3 = row.id;
                    } else {

                    }

                    if(row.variants){

                        //console.log(row.variants);

                        if((row.variants.substr(0, body.word1Text.length) == body.word1Text.toLowerCase() || row.variants.indexOf("/"+body.word1Text.toLowerCase()) != -1) && score1 == 0){
                            score1 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                            col1 = row.id;
                            variant1 = true;
                        } 
                        
                        if((row.variants.substr(0, body.word2Text.length) == body.word2Text.toLowerCase() || row.variants.indexOf("/"+body.word2Text.toLowerCase()) != -1) && score2 == 0){
                            score2 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                            col2 = row.id;
                            variant2 = true;
                        } 
                        
                        if((row.variants.substr(0, body.word3Text.length) == body.word3Text.toLowerCase() || row.variants.indexOf("/"+body.word3Text.toLowerCase()) != -1) && score3 == 0){
                            score3 = this.GetInputScore(words.length, i) * collocation_level_info[0].points_multiplier;
                            col3 = row.id;
                            variant3 = true;
                        }
                    }

                }

            }

            let score = score1 + score2 + score3;

            await this.query.update_collocations_level_score(score, user, type, body.collocationLevelID);

            if(type == 'campaign'){

                let resultLevels = await this.query.get_levels_collocation_campaign_score_sum(user);

                if(resultLevels.length == 1){
                    let updateResult = await this.query.update_collocations_campaign_score(resultLevels[0].sum_score, user);
                }
            }
            
            try{
                await this.query.collocation_log_insert(collocation_level_info[0].id_collocation_level, collocation_level_info[0].level, collocation_level_info[0].structure_id, collocation_level_info[0].headword1, body.word1Text, score1, col1, variant1, body.word2Text, score2, col2, variant2, body.word3Text, score3, col3, variant3, user);
            } catch(e){

            }

            return {score1: score1, score2: score2, score3: score3};        

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    GetInputScore(count_words, i){
        let score = 0;
        let value = 0;

        value = i/count_words;

        if(count_words > 30){

            if (value <= 0.10)
                score = this.GameConfig.SOLO_INSERT_SCORING[0];
            else if (value <= 0.30)
                score = this.GameConfig.SOLO_INSERT_SCORING[1];
            else if (value <= 0.55)
                score = this.GameConfig.SOLO_INSERT_SCORING[2];
            else
                score = this.GameConfig.SOLO_INSERT_SCORING[3];

        } else {
            if (i+1 <= 3)
                score = this.GameConfig.SOLO_INSERT_SCORING[0];
            else if (i+1 <= 6)
                score = this.GameConfig.SOLO_INSERT_SCORING[1];
            else if (i+1 <= 10)
                score = this.GameConfig.SOLO_INSERT_SCORING[2];
            else
                score = this.GameConfig.SOLO_INSERT_SCORING[3];
        }

        if(i+1 <= this.GameConfig.SOLO_INSERT_BONUS_TOP_POSITION){
            score += this.GameConfig.SOLO_INSERT_BONUS_TOP_POSITION_POINTS;
        }
        
        return score;
    }

    async SaveScore(levelID, user, type, body){
        try{

            console.log(body);

            await this.query.update_collocations_level_score(body.score, user, type, body.collocationLevelID);

            if(body.order){
                body.order.forEach(
                    async function(item){
                        await this.query.collocation_save_user_choose_order( body.collocationLevelID, user, type, item['position'], item['collocation_id'], body.log_session);
                    }.bind(this)
                );
            }

            if(type == 'campaign'){

                let resultLevels = await this.query.get_levels_collocation_campaign_score_sum(user);

                if(resultLevels.length == 1){
                    let updateResult = await this.query.update_collocations_campaign_score(resultLevels[0].sum_score, user);
                }
            }

            return {score: body.score, collocationLevelID: body.collocationLevelID };

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    async GetCampaignLeadeboard(user, limit){
        try{
            let mainScore = 0;
            let mainPosition = 1;
            let maxLevel = 1;

            let mainScoreSet = await this.query.get_collocation_campaign_score_position(user);        

            if(mainScoreSet.length == 1){
                mainScore = mainScoreSet[0].col_solo_score;
                mainPosition = mainScoreSet[0].rank_position;
            }

            let scoreLevelSet = await this.query.get_collocation_campaign_level_max(user);

            if(scoreLevelSet.length == 1){
                if(scoreLevelSet[0].max_level){
                    maxLevel = scoreLevelSet[0].max_level;
                } else {
                    maxLevel = 1;
                }
            }

            let leaderBoardSet = await this.query.get_collocation_leaderboard_campaign(limit);

            if(leaderBoardSet.length != 0){
                return {leaderboard : leaderBoardSet, player : {mainScore: mainScore, mainPosition: mainPosition, maxLevel: maxLevel}};
            } else {
                return {leaderboard : [], player : {mainScore: mainScore, mainPosition: mainPosition, maxLevel: maxLevel}};
            }

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    MultiplyScoreValues(arrScores, multiplier){
        const multiScores = arrScores.map(x => x * multiplier);

        return multiScores;
    }

}

module.exports = Collocations;