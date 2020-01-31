import Query from "./Query";
import '@babel/polyfill'
import { createDecipher } from "crypto";

class Synonym2 {

    constructor(query) {
        this.query = query;
    }

    GetDifficultyForLevel(levelID){
        return levelID;
    }

    GetMinimumSynonym(){
        return 3;
    }

    async GetLevelInfo(levelID, user, type){
        try{

            let words = await this.query.get_user_level(user, levelID, type);

            if(words.length == 0){
                let new_words = await this.query.get_random_synonym_words_level(levelID, this.GetMinimumSynonym(), 10);

                console.log(new_words);

                let position_counter = 1;
                for (var i = 0; i < new_words.length; i++) {
                    var row = new_words[i];
                    let inserted_word = await this.query.insert_words_level(user, levelID, type, row.linguistic_unit_id, position_counter);
                    position_counter++;
                }

                words = false;
                words = await this.query.get_user_level(user, levelID, type);

                console.log("Words:");
                console.log(words);

                return words;
            }
            
            console.log("Words:");
            console.log(words);

            return words;

        } catch(e){
            throw e;
        }
        
    }

    async GetLevelScore(levelID, user, type){
        try{
            let levelScore = 0;
            let mainScore = 0;
            let mainPosition = 1;
            let scoreLevelSet = await this.query.get_levels_campaign_score(levelID, user, type);

            if(scoreLevelSet.length == 1){
                levelScore = scoreLevelSet[0].sum_score;
            }

            let mainScoreSet = await this.query.get_main_campaign_score_position(user);        

            if(mainScoreSet.length == 1){
                mainScore = mainScoreSet[0].campaign_score;
                mainPosition = mainScoreSet[0].rank_position;
            }

            return {mainScore: mainScore, levelScore: levelScore, mainPosition: mainPosition};

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

            let mainScoreSet = await this.query.get_main_campaign_score_position(user);        

            if(mainScoreSet.length == 1){
                mainScore = mainScoreSet[0].campaign_score;
                mainPosition = mainScoreSet[0].rank_position;
            }

            let scoreLevelSet = await this.query.get_campaign_level_max(user);

            if(scoreLevelSet.length == 1){
                if(scoreLevelSet[0].max_level){
                    maxLevel = scoreLevelSet[0].max_level;
                } else {
                    maxLevel = 1;
                }
            }

            let leaderBoardSet = await this.query.get_leaderboard_campaign(limit);

            if(leaderBoardSet.length != 0){
                return {leaderboard : leaderBoardSet, player : {mainScore: mainScore, mainPosition: mainPosition, maxLevel: maxLevel}};
            } else {
                return [];
            }

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    async GetCampaignLevel(user){
        try{
            let maxLevel = 1;
            let scoreLevelSet = await this.query.get_campaign_level_max(user);

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


    async CheckWords(levelID, user, type, body){
        try{

            console.log(body);

            let score1 = 0;
            let score2 = 0;
            let score3 = 0;
            let suggestion = "";
            let suggestion_score = 0;
            let suggestion_word_length = 0;

            let words = await this.query.get_synonym_words(body.headwordID);
            
            if(words.length != 0){

                for (var i = 0; i < words.length; i++) {
                    let row = words[i];

                    console.log(row);

                    if(row.text == body.word1Text){
                        score1 = 1;
                    } else if(row.text == body.word2Text){
                        score2 = 1;
                    } else if(row.text == body.word3Text){
                        score3 = 1;
                    } else {

                        if(suggestion == ""){
                            suggestion = row.text;
                            suggestion_score = row.score;
                            suggestion_word_length = row.text.split(" ").length;
                        }

                        if(row.text.split(" ").length < suggestion_word_length){
                                suggestion = row.text;
                                suggestion_score = row.score;
                                suggestion_word_length = row.text.split(" ").length;
                        } else if(row.text.split(" ").length = suggestion_word_length) {
                            if(row.score > suggestion_score){
                                suggestion = row.text;
                                suggestion_score = row.score;
                                suggestion_word_length = row.text.split(" ").length;
                            }
                        }
                    }

                }

            }

            if(score1 == 0){
                this.query.insert_unknown_word(body.headword, body.word1Text);
            }

            if(score2 == 0){
                this.query.insert_unknown_word(body.headword, body.word2Text);
            }

            if(score3 == 0){
                this.query.insert_unknown_word(body.headword, body.word3Text);
            }

            let score = score1 + score2 + score3;

            await this.query.update_words_level_score(score, user, levelID, type, body.headwordID);

            if(type == 'campaign'){

                let resultLevels = await this.query.get_levels_campaign_score_sum(user);

                if(resultLevels.length == 1){
                    let updateResult = await this.query.update_words_campaign_score(resultLevels[0].sum_score * 100, user);
                }

            }

            return {score1: score1, score2: score2, score3: score3, suggestion: suggestion};        

        } catch(e){
            console.log(e);
            throw e;
        }
        
    }

    async CheckLevelWord(headword, synonymword){
        try{

            let words = await this.query.CheckLevelWord(headword, synonymword);

            return words;

        } catch(e){
            console.log(e);

            return [];
        }
        
    }

    async GetUnknownWords(){
        try{
            let csv = "";
            let words = await this.query.get_unknown_words();

            for(var i = 0; i < words.length; i++){
                let word = words[i];
                csv = csv + word.headword + ", " + word.synonym + ", " + word.created + "\n";
            }

            return csv;

        } catch(e){
            console.log(e);

            return "";
        }
        
    }

}

module.exports = Synonym2;