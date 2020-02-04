import Query from "./Query";
import '@babel/polyfill'
import { createDecipher } from "crypto";

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
                    let choose_scores = this.GameConfig.CHOOSE_SCORING;

                    for (var i = 0; i < chooseButtons.length; i++) {
                        var row = chooseButtons[i];

                        let word_object =   {   "choose_position": position_counter, 
                                                "group" : ((position_counter-1)%this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE)+1, 
                                                "score": choose_scores[Math.floor((position_counter-1)/this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE)],
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
                                        "max_round_score": 300,
                                        "next_round": 1,
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.GameConfig.CHOOSE_SCORING,
                                        "bonus_points": this.GameConfig.CHOOSE_BONUS
                                    };

                } else if(collocation_level_info[0].game_type_name == 'insert'){

                    let words = Array({ "word": collocation_level_info[0].headword1, 
                                        "position": collocation_level_info[0].headword_position, 
                                        "structure_text": collocation_level_info[0].text,
                                        "structure_id": collocation_level_info[0].id,
                                        "buttons" : []
                                    });

                    response = {    "words": words,   
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.INSERT_MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.GameConfig.INSERT_SCORING,
                                        "bonus_points": 0
                                    };

                } else if(collocation_level_info[0].game_type_name == 'drag'){

                    let dragWords = await this.query.get_drag_words(collocation_level_info[0].id, collocation_level_info[0].headword1, collocation_level_info[0].headword2, this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND);

                    let position_counter = 1;

                    let words = Array();

                    let random_word_position = Math.floor(Math.random() * Math.floor(dragWords.length)) + 1;

                    let last_word_position = 1;

                    console.log("Random position:"+random_word_position);

                    for (var i = 0; i < dragWords.length; i++) {
                        var row = dragWords[i];

                        if(i == random_word_position){
                            continue;
                        }

                        let response_buttons = [];

                        if(row.nr_distinct == 2){

                            let word_object1 =   {   
                                "score": this.GameConfig.DRAG_SCORING[0], 
                                "word": collocation_level_info[0].headword1
                            };

                            response_buttons.push(word_object1);

                            let word_object2 =   {   
                                "score": this.GameConfig.DRAG_SCORING[0], 
                                "word": collocation_level_info[0].headword2
                            };

                            response_buttons.push(word_object2);

                            let word_object3 =   {   
                                "score": this.GameConfig.DRAG_SCORING[2], 
                                "word": ""
                            };

                            response_buttons.push(word_object3);

                        } else {

                            if(row.headword == collocation_level_info[0].headword1){

                                let word_object1 =   {   
                                    "score": this.GameConfig.DRAG_SCORING[0], 
                                    "word": collocation_level_info[0].headword1
                                };
    
                                response_buttons.push(word_object1);

                                let word_object2 =   {   
                                    "score": this.GameConfig.DRAG_SCORING[2], 
                                    "word": collocation_level_info[0].headword2
                                };
    
                                response_buttons.push(word_object2);
                            }

                            if(row.headword == collocation_level_info[0].headword2){

                                let word_object1 =   {   
                                    "score": this.GameConfig.DRAG_SCORING[0], 
                                    "word": collocation_level_info[0].headword2
                                };
    
                                response_buttons.push(word_object1);

                                let word_object2 =   {   
                                    "score": this.GameConfig.DRAG_SCORING[2], 
                                    "word": collocation_level_info[0].headword1
                                };
    
                                response_buttons.push(word_object2);
                            }

                            let word_object3 =   {   
                                "score": this.GameConfig.DRAG_SCORING[1], 
                                "word": ""
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

                    if((position_counter-1) < this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND){
                        //add unknown words

                        let randomWords = await this.query.get_random_words(this.GameConfig.DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND - (position_counter-1));

                        for (var i = 0; i < randomWords.length; i++) {
                            var row = randomWords[i];
                            
                            let response_buttons = [];

                            let word_object1 =   {   
                                "score": this.GameConfig.DRAG_SCORING[2], 
                                "word": collocation_level_info[0].headword1
                            };

                            response_buttons.push(word_object1);

                            let word_object2 =   {   
                                "score": this.GameConfig.DRAG_SCORING[2], 
                                "word": collocation_level_info[0].headword2
                            };

                            response_buttons.push(word_object2);

                            let word_object3 =   {   
                                "score": this.GameConfig.DRAG_SCORING[0], 
                                "word": ""
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

                    response = {    "words": words,   
                                        "game_type": collocation_level_info[0].game_type_name,
                                        "max_select": this.GameConfig.INSERT_MAX_SELECT,
                                        "number_of_rounds": this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE,
                                        "buttons_number": this.GameConfig.INSERT_NUMBER_OF_WORDS_IN_ONE_ROUND,
                                        "scoring": this.GameConfig.INSERT_SCORING,
                                        "bonus_points": 0
                                    };

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

            let words = await this.query.get_collocation_words(body.collocationLevelID);
            
            if(words.length != 0){

                for (var i = 0; i < words.length; i++) {
                    let row = words[i];

                    //console.log(row);

                    if(row.text == body.word1Text.toLowerCase()){
                        score1 = this.GetInputScore(row.order_value);
                    } else if(row.text == body.word2Text.toLowerCase()){
                        score2 = this.GetInputScore(row.order_value);
                    } else if(row.text == body.word3Text.toLowerCase()){
                        score3 = this.GetInputScore(row.order_value);
                    } else {

                    }

                    if(row.variants){

                        //console.log(row.variants);

                        if((row.variants.substr(0, body.word1Text.length) == body.word1Text.toLowerCase() || row.variants.indexOf("/"+body.word1Text.toLowerCase()) != -1) && score1 == 0){
                            score1 = this.GetInputScore(row.order_value);
                        } 
                        
                        if((row.variants.substr(0, body.word2Text.length) == body.word2Text.toLowerCase() || row.variants.indexOf("/"+body.word2Text.toLowerCase()) != -1) && score2 == 0){
                            score2 = this.GetInputScore(row.order_value);
                        } 
                        
                        if((row.variants.substr(0, body.word3Text.length) == body.word3Text.toLowerCase() || row.variants.indexOf("/"+body.word3Text.toLowerCase()) != -1) && score3 == 0){
                            score3 = this.GetInputScore(row.order_value);
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

            return {score1: score1, score2: score2, score3: score3};        

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    GetInputScore(value){
        let score = 0;

        if (value <= 0.25)
            score = this.GameConfig.INSERT_SCORING[0];
        else if (value <= 0.40)
            score = this.GameConfig.INSERT_SCORING[1];
        else if (value <= 0.55)
            score = this.GameConfig.INSERT_SCORING[2];
        else
            score = this.GameConfig.INSERT_SCORING[3];
        
        return score;
    }

    async SaveScore(levelID, user, type, body){
        try{

            console.log(body);

            await this.query.update_collocations_level_score(body.score, user, type, body.collocationLevelID);

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
                mainScore = mainScoreSet[0].campaign_score;
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

}

module.exports = Collocations;