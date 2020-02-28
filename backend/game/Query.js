import mysql from "mysql";
import util from "util";

let date_to_string = function(date) {
    let str = date.getFullYear() + "-" + (date.getMonth()+1) + "-" + date.getUTCDate() + " " + date.getUTCHours() + ":"
                + date.getUTCMinutes() + ":" + date.getUTCSeconds() + "." + date.getUTCMilliseconds() + "";
    // console.log(date + " date to str: " + str);
    return str;
};

class Query {

    constructor() {

        let host = process.env.IGRA_BESED_DATABASE_HOST || 'localhost';
        let port = process.env.IGRA_BESED_DATABASE_PORT || '3306';
        let user = process.env.IGRA_BESED_DATABASE_USER || 'igrabesed';
        let passwd = process.env.IGRA_BESED_DATABASE_PASSWD || 'igrabesed';
        let schema = process.env.IGRA_BESED_DATABASE_SCHEMA || 'igra_english';

        /*
        let host = process.env.IGRA_BESED_DATABASE_HOST || 'mortar.tovarnaidej.com';
        let port = process.env.IGRA_BESED_DATABASE_PORT || '3306';
        let user = process.env.IGRA_BESED_DATABASE_USER || 'igrabesed_tovarna';
        let passwd = process.env.IGRA_BESED_DATABASE_PASSWD || 'tezkogeslo';
        */

        // console.log("query PASS: ", passwd);

        this.connection = mysql.createConnection({
            host     : host,
            port     : port,
            user     : user,
            password : passwd,
            insecureAuth: true,
            database : schema,
            timezone : 'utc'
        });

        this.connection.connect((err) => {
            if(!err) {
                console.log("Database is connected ...");
                this.connection.query("SELECT count(w.uid) as curr_max from user as w;", (err, result) => {
                    if (result != null && result.length > 0) {
                        this.curr_max_id = result[0].curr_max;
                    } else {
                        this.curr_max_id = 0
                    }
                    console.log("Curr max User ID: " + this.curr_max_id);
                });
            } else {
                console.log("Error when connecting on database ...");
                throw err;
            }
        });
    }

    game_type_id(game_type, callback) {
        this.connection.query("SELECT tt.id FROM task_type AS tt WHERE tt.name = ?;", [game_type], callback);
    }

    next_cycle(game_type_id, callback) {
        let t = new Date();
        t.setUTCMilliseconds(t.getUTCMilliseconds() + 1000);
        // console.log("Query: next cycle: ", date_to_string(t), new Date);
        this.connection.query("SELECT tc.id, tc.from_timestamp, tc.to_timestamp FROM task_cycle AS tc      \
                        WHERE tc.from_timestamp > ?                                                         \
                            AND tc.task_type_id = ?                                                          \
                        ORDER BY tc.from_timestamp limit 1;", [date_to_string(t), game_type_id], callback);
    }

    current_thematic_cycle(callback) {
        let t = new Date();
        t.setUTCMilliseconds(t.getUTCMilliseconds() + 1000);
        let date = date_to_string(t);
        // console.log("Query: next cycle: ", date_to_string(t), new Date);
        this.connection.query("SELECT tc.id, tc.from_timestamp, tc.to_timestamp, t.id as thematic_id, t.name, t.task_type_id FROM task_cycle AS tc      \
                    JOIN thematic as t                                                                  \
                    WHERE t.id = tc.thematic_id                                                         \
                        AND tc.from_timestamp <= ?                                                      \
                        AND tc.to_timestamp > ?                                                         \
                        AND tc.task_type_id = ?                                                         \
                    ORDER BY tc.from_timestamp limit 1;", [date, date, 4], callback);
    }

    next_thematic_cycle(callback) {
        let t = new Date();
        t.setUTCMilliseconds(t.getUTCMilliseconds() + 1000);
        let date = date_to_string(t);
        // console.log("Query: next cycle: ", date_to_string(t), new Date);
        this.connection.query("SELECT tc.id, tc.from_timestamp, tc.to_timestamp, t.id as thematic_id, t.name, t.task_type_id FROM task_cycle AS tc      \
                    JOIN thematic as t                                                                  \
                    WHERE t.id = tc.thematic_id                                                         \
                        AND tc.to_timestamp > ?                                                         \
                        AND tc.task_type_id = ?                                                         \
                    ORDER BY tc.from_timestamp limit 1;", [date, 4], callback);
    }

    past_and_current_thematic_cycles(num_of_cycles, callback) {
        let t = new Date();
        let date = date_to_string(t);
        // console.log("Query: next cycle: ", date_to_string(t), new Date);
        this.connection.query("SELECT tc.id, tc.from_timestamp, tc.to_timestamp, t.id as thematic_id, t.name, t.task_type_id FROM task_cycle AS tc      \
                    JOIN thematic as t                                                                  \
                    WHERE t.id = tc.thematic_id                                                         \
                        AND (tc.to_timestamp < ? OR (tc.from_timestamp <= ? AND tc.to_timestamp > ?))    \
                        AND tc.task_type_id = ?                                                         \
                    ORDER BY tc.from_timestamp desc limit ?;", [date, date, date, 4, num_of_cycles], callback);
    }

    all_tasks(cycle_id, callback) {
        this.connection.query("SELECT t.id, t.position as \"t_position\", w.text, csw.position as \"csw_position\", " +
                            " cs.structure_id as \"structure_id\", s.text as \"structure_text\" FROM task AS t " +
                            "  JOIN collocation_shell AS cs " +
                            "  JOIN collocation_shell_word AS csw " +
                            "  JOIN word AS w " +
                            "  JOIN structure as s " +
                            "  ON cs.id = t.collocation_shell_id " +
                            "    AND csw.collocation_shell_id = t.collocation_shell_id " +
                            "    AND w.id = csw.word_id " +
                            "    AND s.id = cs.structure_id " +
                            "  WHERE t.task_cycle_id = ? " +
                            "  ORDER BY t.position;", [cycle_id], callback);
    }

    all_possible_answers(task_id, get_group_position, callback) {
        let select = "SELECT tpa.id, w.text, tpa.score ";
        if (get_group_position) {
            select += ", tpa.group_position, tpa.choose_position "
        }
        this.connection.query(select + " FROM task_possible_answer AS tpa " +
            "  JOIN possible_answer AS pa " +
            "  JOIN linguistic_unit AS lu " +
            "  JOIN word AS w " +
            "  ON tpa.possible_answer_id=pa.id " +
            "    AND pa.linguistic_unit_id=lu.id " +
            "    AND w.linguistic_unit_id=lu.id " +
            "  WHERE tpa.task_id = ? " +
            "  ;", [task_id], callback);
    }

    find_collocation_score(first_word, second_word, structure_id, callback) {
        this.connection.query("select c.order_value  " +
            "   from word as w " +
            "   join word as w2 " +
            "   join collocation_word as cw " +
            "   join collocation_word as cw2 " +
            "        join collocation as c " +
            "   on cw.collocation_id=cw2.collocation_id and c.id=cw.collocation_id and w.id = cw.word_id  and cw2.word_id=w2.id and w.id!=w2.id " +
            "        where BINARY w.text = ? and BINARY w2.text = ? and c.structure_id = ?;", [first_word.toLowerCase(), second_word.toLowerCase(), structure_id], callback);
    }

    find_synonym_score(first_word, second_word, structure_id, callback) {
        this.connection.query("SELECT s.score "+
        " FROM synonym s " +
        " INNER JOIN word w1 on s.linguistic_unit_id = w1.linguistic_unit_id " +
        " INNER JOIN word w2 on s.linguistic_unit_id_syn = w2.linguistic_unit_id " +
        " WHERE BINARY w1.text = ? and BINARY w2.text = ?;", [first_word.toLowerCase(), second_word.toLowerCase(), structure_id], callback);
    }

    unknown_synonym_word(headword, synonym_word) {
        this.connection.query("INSERT INTO synonym_unknown (headword, synonym, created) VALUES (?, ?, NOW()); ", [headword, synonym_word], (e, r) => {
            if (e)
                console.log("Insert task answer:  error: ", e);
        });
    }

    get_choose_scoreboard(callback) {
        this.connection.query("select u.display_name, u.choose_score " +
            " from user as u  " +
            " order by u.choose_score desc limit 200; ", callback)
    }

    get_choose_score(user_id, callback) {
        this.connection.query("select u.display_name, u.choose_score, " +
            "(SELECT COUNT(*) + 1 FROM user x WHERE x.choose_score > u.choose_score) AS position " +
            " from user as u  " +
            " where u.uid = ? ; ", [user_id], callback)
    }

    get_insert_scoreboard(callback) {
        this.connection.query("select u.display_name, u.insert_score " +
            " from user as u  " +
            " order by u.insert_score desc limit 200; ", callback)
    }

    get_insert_score(user_id, callback) {
        this.connection.query("select u.display_name, u.insert_score, " +
            "(SELECT COUNT(*) + 1 FROM user x WHERE x.insert_score > u.insert_score) AS position " +
            " from user as u  " +
            " where u.uid = ? ; ", [user_id], callback)
    }

    get_synonym_scoreboard(callback) {
        this.connection.query("select u.display_name, u.synonym_score " +
            " from user as u  " +
            " order by u.synonym_score desc limit 200; ", callback)
    }

    get_synonym_score(user_id, callback) {
        this.connection.query("select u.display_name, u.synonym_score, " +
            "(SELECT COUNT(*) + 1 FROM user x WHERE x.synonym_score > u.synonym_score) AS position " +
            " from user as u  " +
            " where u.uid = ? ; ", [user_id], callback)
    }

    get_drag_scoreboard(callback) {
        this.connection.query("select u.display_name, u.drag_score " +
            " from user as u  " +
            " order by u.drag_score desc limit 200; ", callback)
    }

    get_drag_score(user_id, callback) {
        this.connection.query("select u.display_name, u.drag_score, " +
            "(SELECT COUNT(*) + 1 FROM user x WHERE x.drag_score > u.drag_score) AS position " +
            " from user as u  " +
            " where u.uid = ? ; ", [user_id], callback)
    }

    get_sum_scoreboard(callback) {
        this.connection.query("select u.display_name, u.sum_score " +
            " from user as u  " +
            " order by u.sum_score desc limit 200; ", callback)
    }

    get_sum_score(user_id, callback) {
        this.connection.query("select u.display_name, u.sum_score, " +
            "(SELECT COUNT(*) + 1 FROM user x WHERE x.sum_score > u.sum_score) AS position " +
            " from user as u  " +
            " where u.uid = ? ; ", [user_id], callback)
    }

    get_thematic_scoreboard(thematic_id, callback) {
        // console.log("get_thematic_scoreboard", thematic_id);
        this.connection.query("select u.display_name, tu.thematic_score " +
            " from thematic_user as tu " +
            " join user as u " +
            " where u.uid = tu.user_id " +
            " and tu.thematic_id = ?" +
            " order by tu.thematic_score desc limit 200; ", [thematic_id], callback)
    }

    get_thematic_score(user_id, thematic_id, callback) {
        // console.log("get_thematic_score", thematic_id);
        this.connection.query("select u.display_name, tu.thematic_score, " +
            "(SELECT COUNT(*) + 1 FROM thematic_user x WHERE x.thematic_id = ? AND x.thematic_score > tu.thematic_score) AS position " +
            " from thematic_user as tu " +
            " join user as u " +
            " where u.uid = tu.user_id " +
            " and tu.thematic_id = ?" +
            " and u.uid = ? ; ", [thematic_id, thematic_id, user_id], callback)
    }

    update_choose_score(user_id, score) {
        this.update_score("choose_score", user_id, score);
    }

    update_insert_score(user_id, score) {
        this.update_score("insert_score", user_id, score);
    }

    update_drag_score(user_id, score) {
        this.update_score("drag_score", user_id, score);
    }

    update_synonym_score(user_id, score) {
        this.update_score("synonym_score", user_id, score);
    }

    set_thematic_score(user_id, thematic_id, score, position) {
        this.connection.query("REPLACE INTO thematic_user (thematic_id, user_id, thematic_score, thematic_position) " +
                                " values(?, ?, ?, ?);", [thematic_id, user_id, score, position],
                                (error, results, fields) => {
            if (error){
                return console.log("update thematic score failed !!! ", error.message);
            }
        });
    }

    update_score(sql_game_name, uid, score) {
        this.connection.query("UPDATE user as u SET u." + sql_game_name + " = u." + sql_game_name + " + ?, " +
                                "u.sum_score = u.sum_score + ? WHERE u.uid = ?;",
                                [Number(score), Number(score), uid],
                                (error, results, fields) => {
            if (error){
                return console.log("update ", sql_game_name, " score failed !!! ", error.message);
            }
        });
    }

    newUser(callback) {
        this.curr_max_id ++;
        let guest_user = "Guest" + this.curr_max_id;
        this.insertNewUser(guest_user, guest_user, (err, res) => {
            if (err) {
                callback(err, null);
            } else {
                this.getUser(guest_user, callback);
            }
        });
    }

    getUser(uid, callback) {
        if (uid == null) {
            this.newUser(callback);
            return;
        }
        this.connection.query("SELECT * FROM user AS u WHERE u.uid = ?;", [uid], (error, results) => {
            if (error) {
                callback(error, null);
            } else {
                // console.log(results);
                if (results != null && results.length > 0) {
                    callback(false, results[0]);
                } else {
                    this.newUser(callback);
                }
            }
        });
    }

    get_thematic_user(uid, thematic_id, callback) {
        this.connection.query("SELECT * FROM thematic_user AS tu WHERE tu.user_id = ? AND tu.thematic_id = ?;",
                                                [uid, thematic_id], (error, results) => {
            if (error) {
                callback(error, null);
            } else {
                // console.log(results);
                if (results != null && results.length > 0) {
                    callback(false, results[0]);
                } else {
                    callback(false, null);
                }
            }
        });
    }

    insertNewUser (uid, display_name, callback) {
        this.connection.query("INSERT INTO user (uid, display_name) VALUES (?, ?); ", [uid, display_name], callback)
    }

    insert_new_task_user (user_id, task_id, callback) {
        this.connection.query("INSERT INTO task_user (user_id, task_id) VALUES (?, ?); ", [user_id, task_id], callback)
    }

    insertNewWord(word, callback) {
        this.connection.query("INSERT INTO linguistic_unit (id) VALUES (null); ", (err, res) => {
            this.connection.query("INSERT INTO word (text, linguistic_unit_id) VALUES (?, ?); ", [word, res.insertId], callback)
        });
    }

    insert_task_possible_answer(lu_id, task_id, task_user_id) {
        this.connection.query("INSERT INTO possible_answer (linguistic_unit_id) VALUES (?); ", [lu_id], (err, res) => {
            if (err)
                console.log("Insert possible answer:  error: ", err);
            this.connection.query("INSERT INTO task_possible_answer (possible_answer_id, task_id) VALUES (?, ?); ", [res.insertId, task_id], (er, re) => {
                if (er)
                    console.log("Insert task possible answer:  error: ", er);
                this.insert_task_answer(task_user_id, re.insertId);
            });
        });
    }

    insert_task_answer(task_user_id, task_possible_answer_id) {
        this.connection.query("INSERT INTO task_answer (task_user_id, task_possible_answer_id) VALUES (?, ?); ", [task_user_id, task_possible_answer_id], (e, r) => {
            if (e)
                console.log("Insert task answer:  error: ", e);
        });
    }

    insert_task_possible_answer_procedure(task_id, task_user_id, word, task_possible_answer_id) {
        this.connection.query("SELECT w.linguistic_unit_id as lu_id FROM word as w where w.text = (?) ", [word.toLowerCase()], (err, result) => {
            if (err) {
                console.log("Error select lu_id!!", err);
            } else {
                if (result != null && result.length > 0) {
                    let lu_id = result[0].lu_id;
                    if (task_possible_answer_id != null) {
                        this.insert_task_answer(task_user_id, task_possible_answer_id);
                    } else {
                        this.insert_task_possible_answer(lu_id, task_id, task_user_id);
                    }
                } else {
                    this.insertNewWord(word, (err, res) => {
                        if (err) {
                            console.log("Error insert new word!!", err);
                        } else {
                            let lu_id = res.insertId;
                            if (task_possible_answer_id != null) {
                                this.insert_task_answer(task_user_id, task_possible_answer_id);
                            } else {
                                this.insert_task_possible_answer(lu_id, task_id, task_user_id);
                            }
                        }
                    })
                }
            }
        });
    }

    login(uid, callback, display_name=null) {
        let query_str = "SELECT * FROM user AS u WHERE u.uid = ? ";
        let query_arr = [uid];
        if (display_name != null) {
            query_str += " or u.display_name = ?";
            query_arr.push(display_name);
        }
        query_str += ";";
        this.connection.query(query_str, query_arr, (error, results) => {
            if (error) {
                console.log("Login error");
                callback(false);
            } else {
                // console.log(results);
                if (results != null && results.length > 0) {
                    console.log("Login true");
                    callback(true, results[0].display_name, results[0].email);
                } else {
                    console.log("Login false");
                    callback(false);
                }
            }
        });
    }

    register(uid, nickname, age, native_language, email, callback) {
        this.login(uid, (is_already_on_database, username=null) => {
            if (!is_already_on_database) {
                this.connection.query("INSERT INTO user (uid, display_name, age, native_language, email) VALUES (?, ?, ?, ?, ?); ",
                    [uid, nickname, age, native_language, email], (e, r) => {
                        if (e) {
                            console.log("Register new player error: ", e);
                            callback(false);
                        }
                        else {
                            console.log("Register successful");
                            callback(true);
                        }
                    });
            } else {
                console.log("Register not successful");
                callback(false); // je že registriran ... al dam true al false ????
            }
        }, nickname);
    }

    set_email(uid, email, callback) {
        this.connection.query("UPDATE user as u SET u.email = ?  WHERE u.uid = ?; ",
            [email, uid],
            (error, results, fields) => {
                if (error){
                    console.log("set_email  failed !!! ", error.message);
                    callback(false);
                } else {
                    callback(true);
                }
        });
    }

    get_user_level(user_id, level_id, type) {

        let sql =   "SELECT w.linguistic_unit_id, w.text, ul.position, ul.score \
                     FROM user_level as ul \
                     JOIN word as w \
                     WHERE ul.linguistic_unit_id = w.linguistic_unit_id \
                     AND ul.level = ? \
                     AND ul.id_user = ? \
                     AND ul.type = ? ;";

        let args = [level_id, user_id, type];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_user_collocations_level(user_id, level_id, type) {

        let sql =   "SELECT cl.level, cl.headword1, cl.headword2, cl.points_multiplier, tt.name as game_type, ucl.type, ucl.score, ucl.collocation_level_id, ucl.position, clt.title as level_title, clt.next_round \
                     FROM user_col_level ucl \
                     INNER JOIN collocation_level cl ON cl.id_collocation_level = ucl.collocation_level_id \
                     INNER JOIN task_type tt ON tt.id = cl.game_type \
                     LEFT JOIN collocation_level_title clt ON clt.level = cl.level \
                     WHERE cl.level = ? \
                     AND ucl.user_id = ? \
                     AND ucl.type = ?";

        let args = [level_id, user_id, type];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_random_synonym_words_level(difficulty, min_syn, number){
        
        let sql =   "SELECT s.linguistic_unit_id, w.text \
                    FROM synonym s \
                    INNER JOIN word w ON w.linguistic_unit_id = s.linguistic_unit_id \
                    LEFT JOIN user_level ul ON ul.linguistic_unit_id = w.linguistic_unit_id \
                    WHERE s.difficulty = ? AND ul.linguistic_unit_id IS NULL \
                    GROUP BY s.linguistic_unit_id \
                    HAVING count(distinct s.linguistic_unit_id_syn) >= ? ORDER BY RAND() LIMIT ?;";

        sql =   "SELECT s.linguistic_unit_id, w.text \
                    FROM synonym s \
                    INNER JOIN word w ON w.linguistic_unit_id = s.linguistic_unit_id \
                    WHERE s.difficulty = ? \
                    GROUP BY s.linguistic_unit_id \
                    HAVING count(distinct s.linguistic_unit_id_syn) >= ? ORDER BY RAND() LIMIT ?;";

        let args = [difficulty, min_syn, number];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_defined_collocation_level(level){

        let sql =   "SELECT * FROM collocation_level WHERE level = ? and active = 1;";

        let args = [level];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_defined_collocation_byid(id){

        let sql =   "SELECT cl.*, s.*, tt.name as game_type_name \
                FROM collocation_level cl \
                LEFT JOIN structure s ON s.id = cl.structure_id \
                INNER JOIN task_type tt ON tt.id = cl.game_type \
                WHERE  cl.id_collocation_level = ? ;";

        let args = [id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    insert_words_level(user_id, level_id, type, words_id, position){
        
        let sql =   "INSERT INTO user_level (id_user, level, type, linguistic_unit_id, position, score) VALUES (?, ?, ?, ?, ?, -1) ";

        let args = [user_id, level_id, type, words_id, position];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    insert_collocation_user_level(user_id, type, col_level_id, position){
        
        let sql =   "INSERT INTO user_col_level (user_id, type, collocation_level_id, position, score) VALUES (?, ?, ?, ?, -1) ";

        let args = [user_id, type, col_level_id, position];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    insert_unknown_word(headword, synonym){

        if(synonym != ""){
            let sql =   "INSERT INTO synonym_unknown (headword, synonym, created) VALUES (?, ?, NOW())";

            let args = [headword, synonym];
    
            return util.promisify( this.connection.query )
                .call( this.connection, sql, args );        
        } else {
            return true;
        }
    
    }

    update_words_level_score(score, user_id, level_id, type, words_id){
        
        let sql =   "UPDATE user_level SET score = ? WHERE id_user = ? AND level = ? AND type = ? AND linguistic_unit_id = ? ";

        let args = [score, user_id, level_id, type, words_id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    update_words_campaign_score(score, user_id){
        
        let sql =   "UPDATE user SET campaign_score = ? WHERE uid = ?";

        let args = [score, user_id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_main_campaign_scores(user_id){

        let sql = " SELECT campaign_score \
                    FROM user \
                    WHERE uid = ? ;"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_leaderboard_campaign(limit){

        let sql = " SELECT uid, campaign_score, display_name, FIND_IN_SET(campaign_score,(SELECT GROUP_CONCAT( campaign_score ORDER BY campaign_score DESC ) FROM user)) as rank_position, IF(max(ul.level) IS NULL, 0, max(ul.level))  as max_level \
                    FROM user u \
                    LEFT JOIN user_level ul ON ul.id_user = uid AND type = 'campaign' \
                    GROUP BY uid \
                    ORDER BY campaign_score DESC \
                    LIMIT ? ;"

        let args = [limit];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_main_campaign_score_position(user_id){

        let sql = "     SELECT uid, campaign_score, FIND_IN_SET(campaign_score,(SELECT GROUP_CONCAT( campaign_score ORDER BY campaign_score DESC ) FROM user)) as rank_position \
                        FROM user \
                        WHERE uid = ?;"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_levels_campaign_score_sum(user_id){

        let sql = " SELECT sum(score) as sum_score, max(level) as max_level \
                    FROM user_level \
                    WHERE id_user = ? \
                    AND type = 'campaign' \
                    AND score > 0;"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_campaign_level_max(user_id){

        let sql = " SELECT sum(score) as sum_score, max(level) as max_level \
                    FROM user_level \
                    WHERE id_user = ? \
                    AND type = 'campaign';"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_levels_campaign_score(level, user_id, type){

        let sql = " SELECT sum(score) as sum_score \
                    FROM user_level \
                    WHERE id_user = ? \
                    AND level = ? \
                    AND type = ? \
                    AND score > 0;"
        
        let args = [user_id, level, type];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_synonym_words(headwordID){
        
        let sql =   "SELECT w2.text, s.score \
                     FROM synonym s \
                     INNER JOIN word w1 on s.linguistic_unit_id = w1.linguistic_unit_id \
                     INNER JOIN word w2 on s.linguistic_unit_id_syn = w2.linguistic_unit_id \
                     WHERE w1.linguistic_unit_id = ? \
                     ORDER BY RAND();";

        let args = [headwordID];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_choose_words(structureID, headwordText, limit){
        
        let sql =   "SELECT DISTINCT w.linguistic_unit_id, w.text, c.frequency, cw.collocation_id, c.order_value  \
                    FROM collocation_word as cw                                 \
                    JOIN word as w                                              \
                    JOIN collocation as c                                       \
                    JOIN structure as s                                         \
                    ON cw.collocation_id=c.id AND w.id=cw.word_id AND s.id = c.structure_id         \
                    LEFT JOIN collocation_priority cp ON cp.collocation_id = c.id \
                    WHERE cw.position!=s.headword_position AND s.id=? AND w.text != ?              \
                    AND c.id IN (SELECT cw.collocation_id           \
                        FROM collocation_word as cw                                 \
                        JOIN word as w                                              \
                        ON w.id=cw.word_id                                          \
                        WHERE w.text= ?)                                            \
                    ORDER BY cp.priority ASC, c.frequency DESC LIMIT ?;";

        let args = [structureID, headwordText, headwordText, limit];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_drag_words(structureID, headwordID1, headwordID2, limit){
        
        let sql =   "   SELECT  w.text, cw.position, c.order_value, w.linguistic_unit_id, c.frequency, cw.collocation_id, w2.text as headword, count(distinct w2.id) as nr_distinct \
                        FROM collocation c \
                        INNER JOIN collocation_word cw ON cw.collocation_id = c.id \
                        INNER JOIN word w ON w.id= cw.word_id \
                        INNER JOIN collocation_word cw2 ON cw2.collocation_id = c.id \
                        INNER JOIN word w2 ON w2.id= cw2.word_id \
                        INNER JOIN structure s ON s.id = c.structure_id \
                        WHERE s.id = ? \
                        AND w2.text IN (?, ?) \
                        AND cw.position!=s.headword_position  \
                        GROUP BY w.text \
                        ORDER BY RAND() \
                        LIMIT ? ;";

        let args = [structureID, headwordID1, headwordID2, limit];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_unknown_words(){

        let sql = " SELECT * \
                    FROM synonym_unknown"
        
        let args = [];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_random_words(limit){

        let sql = " SELECT DISTINCT w.id, w.text \
                    FROM word as w \
                    ORDER BY RAND() LIMIT ?;";
        
        let args = [limit];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_random_words_structure(structure_id, limit, headword1, headword2){

        let sql = " SELECT  c.id, w.text, wh.text \
                    FROM collocation c \
                    INNER JOIN ( \
                        SELECT c.id \
                        FROM collocation c \
                        INNER JOIN structure s ON s.id= c.structure_id \
                        WHERE c.structure_id = ? \
                        ORDER BY RAND() \
                        LIMIT ? \
                    ) as ran ON ran.id = c.id \
                    INNER JOIN structure s ON s.id = c.structure_id \
                    INNER JOIN collocation_word cw ON cw.collocation_id = c.id AND cw.position != s.headword_position \
                    INNER JOIN collocation_word cwh ON cwh.collocation_id = c.id AND cwh.position = s.headword_position \
                    INNER JOIN word w ON w.id = cw.word_id \
                    INNER JOIN word wh ON wh.id = cwh.word_id \
                    WHERE wh.text NOT IN ( ?, ?) \
                    LIMIT ? "

        let args = [structure_id, limit*100, headword1, headword2, limit];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );

    }

    get_collocation_priority(collocation_id, game_type){

        let sql = " SELECT * FROM collocation_priority WHERE collocation_id = ? and game_type = ?"
        
        let args = [collocation_id, game_type];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    create_collocation_priority(collocation_id, game_type, priority, specific_weight, total_weight, weight_limit){

        let sql = " INSERT INTO collocation_priority (collocation_id, game_type, priority, specific_weight, total_weight, weight_limit) VALUES (?, ?, ?, ?, ?, ?)"
        
        let args = [collocation_id, game_type, priority, specific_weight, total_weight, weight_limit];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    update_collocation_priority(collocation_id, game_type, priority, total_weight){

        let sql = "UPDATE collocation_priority SET priority = ?, total_weight = ? WHERE collocation_id = ? AND game_type = ?";
        
        let args = [priority, total_weight, collocation_id, game_type];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_collocation_campaign_level_max(user_id){

        let sql = " SELECT sum(ucl.score) as score, max(cl.level) as max_level \
                    FROM user_col_level ucl \
                    INNER JOIN collocation_level cl ON ucl.collocation_level_id = cl.id_collocation_level AND cl.active = 1 \
                    WHERE ucl.user_id = ? AND ucl.type = 'campaign';"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
    }

    get_collocation_words(collocationLevelID){
        
        let sql =   "   SELECT c.id, c.order_value, ww.text, ww.variants \
                        FROM collocation_level cl \
                        INNER JOIN word w ON cl.headword1 = w.text \
                        LEFT JOIN collocation_word cw ON cw.word_id = w.id \
                        LEFT JOIN collocation c ON cw.collocation_id = c.id AND c.structure_id = cl.structure_id \
                        LEFT JOIN collocation_word cww ON cww.collocation_id = c.id AND cww.word_id != cw.word_id \
                        LEFT JOIN word ww ON ww.id = cww.word_id \
                        WHERE cl.id_collocation_level = ? AND cl.active = 1\
                        HAVING c.id IS NOT NULL \
                        ORDER BY c.order_value ASC;";

        let args = [collocationLevelID];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    update_collocations_level_score(score, user_id, type, level_id){
        
        let sql =   "UPDATE user_col_level SET score = ? WHERE user_id = ? AND type = ? AND collocation_level_id = ? ";

        let args = [score, user_id, type, level_id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    get_user_collocations_score_level(user, level, type){
        let sql =   "   SELECT SUM(score) as sum_score \
                        FROM user_col_level ucl \
                        INNER JOIN collocation_level cl ON cl.id_collocation_level = ucl.collocation_level_id AND cl.active = 1 \
                        WHERE ucl.user_id = ? AND cl.level = ? AND ucl.type = ? AND score > 0 ";

        let args = [user, level, type];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );         
    }

    get_collocation_campaign_score_position(user_id){

        let sql = "     SELECT uid, col_solo_score, FIND_IN_SET(col_solo_score,(SELECT GROUP_CONCAT( col_solo_score ORDER BY col_solo_score DESC ) FROM user)) as rank_position \
                        FROM user \
                        WHERE uid = ?;"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_collocation_campaign_level_max(user_id){

        let sql = " SELECT sum(ucl.score) as sum_score, max(cl.level) as max_level \
                    FROM user_col_level ucl\
                    INNER JOIN collocation_level cl ON cl.id_collocation_level = ucl.collocation_level_id AND cl.active = 1 \
                    WHERE user_id = ? \
                    AND type = 'campaign';"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_collocation_leaderboard_campaign(limit){

        let sql = " SELECT uid, col_solo_score, display_name, FIND_IN_SET(col_solo_score,(SELECT GROUP_CONCAT( col_solo_score ORDER BY col_solo_score DESC ) FROM user)) as rank_position, IF(max(cl.level) IS NULL, 0, max(cl.level))  as max_level \
                    FROM user u \
                    INNER JOIN user_col_level ucl ON ucl.user_id = uid AND type = 'campaign' \
                    INNER JOIN collocation_level cl ON cl.id_collocation_level = ucl.collocation_level_id AND cl.active = 1 \
                    GROUP BY uid \
                    ORDER BY col_solo_score DESC \
                    LIMIT ? ;"

        let args = [limit];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    get_levels_collocation_campaign_score_sum(user_id){

        let sql = " SELECT sum(u.score) as sum_score, max(cl.level) as max_level \
                    FROM user_col_level u\
                    INNER JOIN collocation_level cl ON cl.id_collocation_level = u.collocation_level_id AND cl.active = 1 \
                    WHERE user_id = ? \
                    AND type = 'campaign' \
                    AND score > 0;"
        
        let args = [user_id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    update_collocations_campaign_score(score, user_id){
        
        let sql =   "UPDATE user SET col_solo_score = ? WHERE uid = ?";

        let args = [score, user_id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }
       
}

module.exports = Query;