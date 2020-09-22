import express from "express";
import '@babel/polyfill'
import Query from "./game/Query";
import fs from "fs";
import https from "https";
import http from "http";
import * as admin from 'firebase-admin';
//let serviceAccount = require('../igra-besed-firebase-adminsdk-xjz06-48665ca358.json');
let serviceAccount = require('../test-d6087-d92edc8fdb30.json');
import bodyParser from "body-parser";
import ChooseTypeRoom from "./game/ChooseTypeRoom";
import DragTypeRoom from "./game/DragTypeRoom";
import InsertTypeRoom from "./game/InsertTypeRoom";
import SynonymTypeRoom from "./game/SynonymTypeRoom";
import Synonym2 from "./game/Synonym2";
import Collocations from "./game/Collocations";
import AdminPage from "./admin/AdminPage";
import AdminTokenMiddleware from "./admin/middleware/AuthToken";
import ThematicRoom from "./game/ThematicRoom";
import default_room from "./game/Room";
import multer from "multer";

// TODO:
//var session = require('express-session');

let date_to_string = function(date) {
	return date.getUTCHours() + ":" + date.getUTCMinutes() + ":" + date.getUTCSeconds() + "." + date.getUTCMilliseconds();
};

let game_conf_file = process.env.IGRA_BESED_GAME_CONF_JSON || 'GameConfig.json';
let game_conf = JSON.parse(fs.readFileSync(game_conf_file, 'utf8'));

let query = new Query();
let choose_type_room = new ChooseTypeRoom(query, game_conf);
let drag_type_room = new DragTypeRoom(query, game_conf);
let insert_type_room = new InsertTypeRoom(query, game_conf);
let thematic_room = new ThematicRoom(query, game_conf);
let synonym_type_room = new SynonymTypeRoom(query, game_conf);

let synonym_new = new Synonym2(query);

let collocations_new = new Collocations(query, game_conf);

let admin_page = new AdminPage(query);

function switch_room(type) {
	console.log("Switch room: ", type);
	if (type == "choose") return choose_type_room;
	if (type == "drag") return drag_type_room;
    if (type == "insert") return insert_type_room;
    if (type == "synonym") return synonym_type_room;
	if (type == "thematic") return thematic_room;
}

var app = express();

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

app.use(function(req, res, next) {
	res.header("Access-Control-Allow-Origin", "*");
	res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
	next();
});

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    //databaseURL: "https://igra-besed.firebaseio.com"
    databaseURL: "https://test-d6087.firebaseio.com/"
});

function authentication(user_token) {
    /*
    return new Promise((resolve, reject) => {
        let uid = "F18OwBMqA0b1CnVp9VdTvlsVnZW2";
        resolve(uid);
    });
    */
    return new Promise((resolve, reject) => {
        if (user_token.startsWith("Guest") || user_token.startsWith("Gost")) resolve(null);

        admin.auth().verifyIdToken(user_token)
            .then(function (decodedToken) {
                let uid = decodedToken.uid;
                // console.log("token: ", user_token);
                // ...
                resolve(uid);
            }).catch(function (error) {
                // Handle error
                console.log("authentication error: ");
                resolve(null);
        });
    });
}

var router = express.Router();
//route to handle user registration
app.post('/api/v1/register', async function(req, res) {
    let user_token = req.body.user_id;
    console.log("register");
    let uid = await authentication(user_token);
    if (uid == null) {
        res.send({
            success: false
        });
        return;
    }

    let nickname = req.body.nickname;
    let age = req.body.age;
    let native_language = req.body.native_language;
    // console.log("User id register: " + uid + " time: " +  date_to_string(new Date())
    //         + " nick: " + nickname + " age: " + age + " lang: " + native_language);

    admin.auth().getUser(uid)
        .then(function(userRecord) {
            let email = userRecord.providerData[0].email;
            console.log('Successfully fetched user data:', email);

            query.register(uid, nickname, age, native_language, email, (success) => {
                console.log('success:', success);
                res.send({
                    success: success
                })
            });
        })
        .catch(function(error) {
            console.log('Error fetching user data:', error);
            res.send({
                success: false
            });
        });
});

app.post('/api/v1/login', async function(req, res) {
    let user_token = req.body.user_id;
    console.log("login");
    let uid = await authentication(user_token);
    if (uid == null) {
        res.send({
            success: false
        });
        return;
    }
    // console.log("User id login: " + uid + " time: " +  date_to_string(new Date()));

    query.login(uid, (is_on_database, username=null, email=null) => {
        if (is_on_database) {
            thematic_room.newPlayer_next_cycle(uid, (error, json) => {
                //console.log(response)
                if (json == null) json = {};
                json.display_name = username;
                json.email = email;
                json.success = true;
                res.send(json);
            });
        } else {
            res.send({
                display_name: username,
                email: email,
                success: false
            })
        }
    })
});

app.post('/api/v1/set_email', async function(req, res) {
    let user_token = req.body.user_id;
    let email = req.body.email;
    console.log("set_email");
    let uid = await authentication(user_token);
    if (uid == null) {
        res.send({
            success: false
        });
        return;
    }

    query.set_email(uid, email, (success) => {
        res.send({
            success: success
        });
    });
});

app.get('/api/v1/thematic_scoreboard', async function(req, res) {
    let user_token = req.query.user_id;
    let thematic_id = req.query.thematic_id;
    console.log("thematic scoreboard");
    let uid = await authentication(user_token);
    // console.log("token: ", uid);
    if (uid == null && user_token.startsWith("Guest")) uid = user_token;
    let room = thematic_room;
    // console.log("User id wanted scoreboard: " + uid + " time: " +  date_to_string(new Date()));

    let scoreboard = null;
    if (room != null) {
        scoreboard = await room.get_global_scoreboard(uid, thematic_id);
        // console.log("global scoreboard:");
        // console.log(scoreboard);

        if (scoreboard != null) {
            res.send(scoreboard);
            return;
        }
    }
    res.send({
        "code" : 400,
        "failure" : "get global scoreboard failed"
    });
});

app.get('/api/v1/global_scoreboard', async function(req, res) {
    let user_token = req.query.user_id;
    console.log("global scoreboard");
    let uid = await authentication(user_token);
    // console.log("token: ", uid);
    if (uid == null && user_token.startsWith("Guest")) uid = user_token;
    let room = switch_room(req.query.type);
    // console.log("User id wanted scoreboard: " + uid + " time: " +  date_to_string(new Date()));

    var scoreboard = null;
    if (room != null) {
        scoreboard = await room.get_global_scoreboard(uid);
        // console.log("global scoreboard:");
        // console.log(scoreboard);

        if (scoreboard != null) {
            res.send(scoreboard);
            return;
        }
    }
    if (req.query.type == 'sum') {
        scoreboard = await new Promise(resolve => {
            default_room.Room.get_global_scoreboard_func(uid,
                'sum_score',
                query.get_sum_scoreboard.bind(query),
                query.get_sum_score.bind(query),
                resolve);
        });

        if (scoreboard != null) {
            res.send(scoreboard);
            return;
        }
    }
    res.send({
        "code" : 400,
        "failure" : "get global scoreboard failed"
    });
});

router.post('/publish_score', async function (req, res) {
    let user_token = req.query.user_id;
    let uid = await authentication(user_token);
    if (uid == null && user_token.startsWith("Guest")) uid = user_token;
    console.log("publish score: ", uid);

    let cycle_id = req.body.cycle_id;
    let room = switch_room(req.query.type);
    let data = null;
    if (room != null) {
        console.log("User wanted to set score: time: " + date_to_string(new Date()));

        data = await room.set_score_for_player(uid, cycle_id, req.body);
        console.log("User got set score: " + data);
    }

    if (data != null) {
        // console.log("Send success score: " + uid + " score: " + st, res);
        res.send({
            "code": 200,
            "data": data
        })
    }
    else {
        // console.log("Send error score: " + uid + " score: " + st, res);
        res.send({
            "code": 400,
            "failure": "publish score failed"
        })
    }
});

app.get('/api/v1/scoreboard', function(req, res) {
    // let user_id = req.query.user_id;
    let cycle_id = req.query.cycle_id;
    let room = switch_room(req.query.type);
	console.log("User id wanted scoreboard: <id>" + " time: " +  date_to_string(new Date()));
    let scoreboard = room.post_scoreboard(cycle_id);
	// console.log("scoreboard:");
	// console.log(scoreboard);
    if (scoreboard != null) {
        res.send({
            "code" : 200,
            "cycle_id" : cycle_id,
			"scoreboard" : scoreboard
        })
    }
    else {
        res.send({
            "code" : 400,
			"failure" : "get scoreboard failed"
        })
    }
});

app.get('/api/v1/connect', async function(req, res) {
    let user_token = req.query.user_id;
    let uid = await authentication(user_token);
	let room = switch_room(req.query.type);
	console.log("User wanted to connect: time: " +  date_to_string(new Date()) + " " + req.query.type);

	room.newPlayer_next_cycle(uid, (error, json) => {
        //console.log(response)
        res.send(json);
	});
});

app.get('/api/v1/connect_immediate', async function(req, res) {
    let user_token = req.query.user_id;
    let uid = await authentication(user_token);
    let room = switch_room(req.query.type);
	console.log("User wanted to connect immediately: time: " +  date_to_string(new Date()));

    room.newPlayer_immediate(uid, (error, json) => {
        //console.log(response)
        res.send(json);
    });
});

app.get('/api/v1/level_info', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let words = await synonym_new.GetLevelInfo(level_id, uid, type);

    res.send(JSON.stringify({words: words}));
});

app.get('/api/v1/level_score', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let score = await synonym_new.GetLevelScore(level_id, uid, type);

    res.send(JSON.stringify(score));
});

app.get('/api/v1/level_campaign', async function(req, res) {
    let user_token = req.query.user_id;
    //let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let level = await synonym_new.GetCampaignLevel(uid);

    res.send(JSON.stringify(level));
});

app.get('/api/v1/leaderboard_campaign', async function(req, res) {
    let user_token = req.query.user_id;
    //let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let leaderboard = await synonym_new.GetCampaignLeadeboard(uid, 25);

    res.send(JSON.stringify(leaderboard));
});

app.post('/api/v1/level_check', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await synonym_new.CheckWords(level_id, uid, type, req.body);

    res.send(JSON.stringify(response));
});

app.get('/api/v1/csv_export', async function(req, res) {
    let password = req.query.password;

    if(password == 'sPWptXEuJ6L4TKdW'){
        let response = await synonym_new.GetUnknownWords();
        res.setHeader('Content-Type', 'text/csv');
        res.setHeader('Content-Disposition', 'attachment; filename=\"' + 'unknown-synonyms-' + Date.now() + '.csv\"');
        res.send(response);
    } else {
        res.send("Authorization failed!");
    }
});

//Solo collocations

app.get('/api/v1/col/level_info', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.GetLevelInfo(level_id, uid, type);

    if(response.error){
        res.status(500).send(JSON.stringify(response));
    } else {
        res.send(JSON.stringify(response));
    }
});

app.get('/api/v1/col/game_info', async function(req, res) {
    let user_token = req.query.user_id;
    let game_id = req.query.game_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.GetGameInfo(game_id, uid, type);

    if(response.error){
        res.status(500).send(JSON.stringify(response));
    } else {
        res.send(JSON.stringify(response));
    }
});

app.post('/api/v1/col/choose/set_weight', async function(req, res) {
    let user_token = req.query.user_id;
    let game_type = req.query.game_type;
    let collocation_id = req.body.collocation_id;
    let game_mode = req.body.game_mode;
    let level_id = req.query.level_id;
    let game_id = req.query.game_id;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.IncreaseCollocationWeight(collocation_id, game_mode);

    let response_log = await collocations_new.LogChoose(game_id, uid, req.body);

    if(response.error){
        res.status(500).send(JSON.stringify(response));
    } else {
        res.send(JSON.stringify(response));
    }
});

app.post('/api/v1/col/drag/log', async function(req, res) {
    let user_token = req.query.user_id;
    let game_type = req.query.game_type;
    let level_id = req.query.level_id;
    let game_id = req.query.game_id;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.LogDrag(game_id, uid, req.body);

    if(response.error){
        res.status(500).send(JSON.stringify(response));
    } else {
        res.send(JSON.stringify(response));
    }
});

app.get('/api/v1/col/level_campaign', async function(req, res) {
    let user_token = req.query.user_id;
    //let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let level = await collocations_new.GetCampaignLevel(uid);

    res.send(JSON.stringify(level));
});

app.post('/api/v1/col/insert/level_check', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.CheckInputWords(level_id, uid, type, req.body);

    res.send(JSON.stringify(response));
});

app.post('/api/v1/col/drag/save_score', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.SaveScore(level_id, uid, type, req.body);

    res.send(JSON.stringify(response));
});

app.post('/api/v1/col/choose/save_score', async function(req, res) {
    let user_token = req.query.user_id;
    let level_id = req.query.level_id;
    let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let response = await collocations_new.SaveScore(level_id, uid, type, req.body);

    res.send(JSON.stringify(response));
});

app.get('/api/v1/col/leaderboard_campaign', async function(req, res) {
    let user_token = req.query.user_id;
    //let type = req.query.type;

    let uid = await authentication(user_token);
    if (uid == null && ( user_token.startsWith("Guest") || user_token.startsWith("Gost"))) uid = user_token;

    let leaderboard = await collocations_new.GetCampaignLeadeboard(uid, 25);

    res.send(JSON.stringify(leaderboard));
});

app.get('/api/v1/game/modes', async function(req, res) {

    let gamemodes = await admin_page.gamemodesInfoGet();
    let response = {collocations: {solo: gamemodes['collocations_solo'] == 1 ? true : false, multi: gamemodes['collocations_multiplayer'] == 1 ? true : false}, synonyms: {solo: gamemodes['synonyms_solo'] == 1 ? true : false, multi: gamemodes['synonyms_multiplayer'] == 1 ? true : false}};

    if(response.error){
        res.status(500).send(JSON.stringify(response));
    } else {
        res.send(JSON.stringify(response));
    }
});

app.get('/api/v1/language', async function(req, res) {
    let code = req.query.code;

    try{
        let response = JSON.parse(fs.readFileSync("translations/"+code+'.json', 'utf8'));

        res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/users/list', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin'){
            let response = {};
            response.data = await admin_page.listUsers();
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/users/add', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin'){
            let response = {};
            response.data = await admin_page.saveUser(req.body);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins!");
        }

    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/users/edit', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin'){
            let response = {};
            response.data = await admin_page.editUser(req.query.id, req.body);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/users/info', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{

        let response = {};
        response.data = req.user;
        response.code = 20000;

        res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/users/get', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{

        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.userInfoGet(req.query.id);
            response.code = 20000;
    
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/users/login', async function(req, res) {
    let username = req.body.username;
    let password = req.body.password;

    try{

        let response = await admin_page.checkLogin(username, password);

        res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/users/logout', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
            let response = {};
            response.data = {message: "OK"};
            response.code = 20000;
            res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/structures/list', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.listStructures();
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});


app.get('/api/v1/admin/structures/get', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{

        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.structureInfoGet(req.query.id);
            response.code = 20000;
    
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/structures/delete', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{

        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.structureDeleteGet(req.query.id);
            response.code = 20000;
    
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/structures/save', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.saveStructure(req.query.id, req.body);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/imports/list', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    let page = req.query.page;
    let limit = 15;

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.listImports(limit, (page-1)*limit);
            response.page = page;
            response.limit = limit;
            response.total = await admin_page.countImports();
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/imports/save', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.saveImport(req.query.id, req.body, req.user);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/imports/logs', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.listImportLogs(req.query.id);
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/imports/get', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{

        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.importsInfoGet(req.query.id);
            response.code = 20000;
    
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/exports/list', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    let page = req.query.page;
    let limit = 15;

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.listExports(limit,(page-1)*limit);
            response.page = page;
            response.limit = limit;
            response.total = await admin_page.countExports();
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/exports/save', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.saveExport(req.query.id, req.body, req.user);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});


app.get('/api/v1/admin/exports/file', AdminTokenMiddleware.checkAdminToken, async function(req, res) {
    let filename = req.query.filename;

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){

            let response = fs.readFileSync("exports/"+filename, 'utf8');

            res.setHeader('Content-Type', 'text/csv');
            res.setHeader('Content-Disposition', 'attachment; filename=\"' + filename + '\"');
            res.send(response);

        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/translations/get', async function(req, res) {
    let code = req.query.code;

    try{
        let response = {};
        response.data = JSON.parse(fs.readFileSync("translations/"+code+'.json', 'utf8'));
        response.code = 20000;

        res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/translations/save', async function(req, res) {
    let code = req.query.code;

    try{
        let response = {};
        let result = fs.writeFileSync("translations/"+code+'.json', req.body.json_file);
        response.data = {"status": "OK"};
        response.code = 20000;

        res.send(JSON.stringify(response));
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/crons/list', AdminTokenMiddleware.checkAdminToken, async function(req, res) {
    let type = req.query.type;

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            response.data = await admin_page.listCrons(type);
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.get('/api/v1/admin/gamemodes/get', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin'){
            let response = {};
            response.data = await admin_page.gamemodesInfoGet();
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});

app.post('/api/v1/admin/gamemodes/save', AdminTokenMiddleware.checkAdminToken, async function(req, res) {

    try{
        if(req.user.role == 'admin'){
            let response = {};
            response.data = await admin_page.saveGamemodes(req.body);
            response.code = 20000;

            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }
});
// FILE UPLOADS
const imports = multer({ dest: 'uploads/imports/' });

app.post('/api/v1/admin/imports/upload', imports.single("importcsv"), AdminTokenMiddleware.checkAdminToken, async function (req, res, next) {

    try{
        if(req.user.role == 'admin' || req.user.role == 'editor'){
            let response = {};
            req.body.fileondisk = req.file.path;
            req.body.status = "uploaded";
            response.data = await admin_page.saveImportFile(req.body.id, req.body);
            response.code = 20000;
            res.send(JSON.stringify(response));
        } else {
            throw new Error("Only for admins/editors!");
        }
    } catch(e){
        res.status(500).send(JSON.stringify({"error":true, "message": e.message}));
    }

})


// CORS HEADERS
app.use(function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*"); // update to match the domain you will make the request from
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
    next();
});

app.use('/api/v1', router);

let server_key = process.env.IGRA_BESED_SERVER_KEY || 'server.key';
let server_cert = process.env.IGRA_BESED_SERVER_CERT || 'server.cert';
let server_address = process.env.IGRA_BESED_SERVER_ADDRESS || '127.0.0.1';
let server_port = process.env.IGRA_BESED_SERVER_PORT || 3000;

http.createServer(app)
    .listen(server_port, server_address, function() {
        console.log("Server is on!")
});

/*https.createServer({
        key: fs.readFileSync(server_key),
        cert: fs.readFileSync(server_cert)
    },
    app)
        .listen(server_port, function() {
	        console.log("Server is on!")
    });
*/