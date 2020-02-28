

using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameSettings
{
    public const float TIME_MS_TO_S = 1000f;

    public const int MAX_PLAYERS_LEADERBOARDS = 300;

    public static string CURRENT_MODE;
    public static bool SOLO;
    public static bool THEMATIC;
    public static int PREVIOUS_MODE = 0;
    public static int ANOTHER_PREVIOUS_MODE = 0;

    // Scene numbers from unity builds settings
    public const int INTRO_SCREEN = 0;
    public const int COMPETITIVE_MODE_SELECTION_MENU = 18;
    public const int MODE_SELECTOR = 26;
    public const int REGISTER = 27;
    public const int SOLO_MODE_CHOOSER = 28;
    public const int LEADERBOARD_ALLTIME = 29;
    public const int MENU_LEADERBOARDS = 30;
    public const int THEMATIC_LEADERBOARDS = 31;
    public const int SEND_EMAIL = 34;

    // Scene numbers for game mode - choose
    public const int WAIT_A_CHOOSE_INFO_CHOOSE = 1;
    public const int SHOW_WORD_CHOOSE = 2;
    public const int CHOOSE_CHOOSE = 3;
    public const int CLASSIFY_INFO_CHOOSE = 4;
    public const int CLASSIFY_CHOOSE = 5;
    public const int ROUND_SCORE_CHOOSE = 6;
    public const int SCOREBOARD_CHOOSE = 7;

    // Scene numbers for game mode - insert
    public const int WAIT_A_INSERT_INFO_INSERT = 8;
    public const int SHOW_WORD_INSERT = 9;
    public const int INSERT_INSERT = 10;
    public const int ROUND_SCORE_INSERT = 11;
    public const int SCOREBOARD_INSERT = 12;

    // Scene numbers for game mode - insert
    public const int WAIT_A_DRAG_INFO_DRAG = 13;
    public const int SHOW_WORD_DRAG = 14;
    public const int DRAG_DRAG = 15;
    public const int ROUND_SCORE_DRAG = 16;
    public const int SCOREBOARD_DRAG = 17;

    // Scene numbers for game settings
    public const int SETTINGS_MENU = 20;
    public const int SETTINGS_ABOUT_POLICY = 21;
    public const int SETTINGS_ABOUT_INSERT = 22;
    public const int SETTINGS_ABOUT_GAME = 23;
    public const int SETTINGS_ABOUT_DRAG = 24;
    public const int SETTINGS_ABOUT_CHOOSE = 25;
    public const int SETTINGS_ABOUT_SCORING = 32;
    public const int SETTINGS_ABOUT_THEMATIC = 33;
    public const int SETTINGS_ABOUT_SYNONYM = 34;

    // Scene numbers for game mode - synonym
    public const int WAIT_A_INSERT_INFO_SYNONYM = 35;
    public const int SHOW_WORD_SYNONYM = 36;
    public const int SYNONYM_SYNONYM = 37;
    public const int ROUND_SCORE_SYNONYM = 38;
    public const int SCOREBOARD_SYNONYM = 39;

    // Scene numbers for game mode - synonym2
    public const int SCENE_ROOM_PICKER = 45;
    public const int SCENE_GAME_MODE = 41;
    public const int SCENE_LEVEL_PICKER = 44;
    public const int SCENE_FILLER_LEVEL = 43;
    public const int SCENE_FILLER_WORD = 46;
    public const int SCENE_INPUT_SYNONYM = 47;
    public const int SCENE_LEVEL_SCORE = 48;

    // Scene numbers for game mode - collocations
    public const int SCENE_ROOM_PICKER_COL = 58;
    public const int SCENE_GAME_MODE_COL = 55;
    public const int SCENE_LEVEL_PICKER_COL = 57;
    public const int SCENE_FILLER_LEVEL_COL = 56;
    public const int SCENE_FILLER_WORD_COL = 59;
    //public const int SCENE_INPUT_SYNONYM_COL = 47;
    public const int SCENE_LEVEL_SCORE_COL = 60;

    public const int SCENE_CHOOSE_COL = 61;
    public const int SCENE_INSERT_COL = 62;
    public const int SCENE_DRAG_COL = 63;

    public const int CLASSIFY_INFO_CHOOSE_COL = 64;
    public const int CLASSIFY_CHOOSE_COL = 65;
    public const int ROUND_SCORE_CHOOSE_COL = 66;

    public const int ROUND_SCORE_INSERT_COL = 67;
    public const int ROUND_SCORE_DRAG_COL = 68;

    public const int SHOW_WORD_CHOOSE_SOLO = 69;
    public const int SHOW_WORD_DRAG_SOLO = 70;
    public const int SHOW_WORD_INSERT_SOLO = 71;

    public const int SCENE_LANGUAGE_SELECT = 72;
    public const int SCENE_LANGUAGE_LOAD = 73;


    // Game mode settings
    public const string GAME_MODE_CHOOSE = "choose";
    public const string GAME_MODE_INSERT = "insert";
    public const string GAME_MODE_DRAG = "drag";
    public const string GAME_MODE_THEMATIC = "thematic";
    public const string GAME_MODE_SYNONYM = "synonym";
    public const string MODE_SUM = "sum";

    public const int SYNONYM_MAX_LEVELS = 500;
    public const int COLLOCATIONS_SOLO_MAX_LEVELS = 500;

    public static string CURRENTLY_RUNNING_GAME_MODE = "default";

    // Colors
    public static Color COLOR_BLUE = new Color(3 / 255f, 169 / 255f, 244 / 255f);
    public static Color COLOR_GRAY = new Color(193 / 255f, 200 / 255f, 210 / 255f);
    public static Color COLOR_RED = new Color(244 / 255f, 67 / 255f, 54 / 255f);
    public static Color COLOR_WHITE = new Color(1, 1, 1);
    public static Color COLOR_DARK_BORDER = new Color(63 / 255f, 72 / 255f, 90 / 255f);

    // Connection settings
    public const int IMMEDIATE = 1; // Connect to current game
    public const int NEXT = 2; // Connect to next game

    // URL endpoints
    public const string URL_END_TYPE = "&type=";
    public const string URL_END_CYCLE = "&cycle_id=";

    // User settings
    public static string username = "Gost";
    public static string email = "";
    public static string user_id = "?";
    public static FirebaseUser user;
    public static string usertype = "";

    // competitive settings
    public static bool thematicInfoExists;
    public static bool activateThematicMode;
    public static string thematicName;
    public static string fromToDate;
    public static int numberOfRounds;
    public static int numberOfPlayedRounds;
    public static bool midGameThematicChange;
    public static bool currentThematic;
    // competitive leaderboard
    public static ThematicInfoRes[] past_and_current_games_info;

    public static bool fromGameSettings = false;

    public static string GetUserFBToken()
    {
        if (user == null)
        {
            user_id = username;
            return user_id;
        }

        return user_id;
    }

    static GameSettings() {
        for (int i = 0; i < 6; i++)
        {
            username += "" + Random.Range(0, 9);
        }
    }

    private static bool test = true;
    public static void MyDebug(string log)
    {
        //if (test)
        //{
            Debug.Log(log);
        //}
    }

    public static LocalizationManager localizationManager;
    public static bool translationLoaded = false;
    public static bool resetLanguage = false;
    public static string defaultLanguageCode = "en";
    public static Sprite languageSprite;

    private static string languageCode;

    //Development environments
    private static string ENV_local = "local";
    private static string ENV_mortar = "mortar";
    private static string ENV_prod = "production";
    private static string ENVIRONMENT = GameSettings.ENV_local;


    public static void SetLanguage(string language_code)
    {
        languageCode = language_code;
        Debug.Log(language_code);

        LoadServerURLs();

        if (servers.ContainsKey(language_code)) {
            if(GameSettings.ENVIRONMENT == GameSettings.ENV_local){
                API_CONNECTION = "http://" + servers[language_code] + "/api/v1/";
            } else {
                API_CONNECTION = "https://" + servers[language_code] + "/api/v1/";
            }
            MyDebug("SET SERVER URL: " + API_CONNECTION);
            refreshURLs();
        }
    }

    public static void LoadServerURLs() {
        servers = new Dictionary<string, string>();

        if (GameSettings.ENVIRONMENT == GameSettings.ENV_local)
        {
            servers.Add("sl", "127.0.0.1:3000");
            servers.Add("de", "127.0.0.1:3000");
            servers.Add("nl", "127.0.0.1:3000");
            servers.Add("pt", "127.0.0.1:3000");
            servers.Add("ee", "127.0.0.1:3000");
            servers.Add("en", "127.0.0.1:3000");

        } else if (GameSettings.ENVIRONMENT == GameSettings.ENV_mortar) {
            
            servers.Add("sl", "sl.igrabesed.dev.mortar.tovarnaidej.com:443");
            servers.Add("de", "de.igrabesed.dev.mortar.tovarnaidej.com:443");
            servers.Add("nl", "nl.igrabesed.dev.mortar.tovarnaidej.com:443");
            servers.Add("pt", "pt.igrabesed.dev.mortar.tovarnaidej.com:443");
            servers.Add("ee", "ee.igrabesed.dev.mortar.tovarnaidej.com:443");
            servers.Add("en", "en.igrabesed.dev.mortar.tovarnaidej.com:443");

        }
    }

    public static void refreshURLs() {
        GETConnectURL = API_CONNECTION + "connect?user_id=";
        GETConnectURLSolo = API_CONNECTION + "connect?user_id=";
        GETConnectImmediateURL = API_CONNECTION + "connect_immediate?user_id=";
        GETConnectImmediateURLSolo = API_CONNECTION + "connect_immediate?user_id=";
        GETScoreboardURL = API_CONNECTION + "scoreboard?user_id=";
        GETScoreboardGlobalURL = API_CONNECTION + "global_scoreboard?user_id=";
        GETScoreboardGlobalThematicURL = API_CONNECTION + "thematic_scoreboard?user_id=";


        GETLevelInfoURL = API_CONNECTION + "level_info?user_id=";
        GETLevelScoreURL = API_CONNECTION + "level_score?user_id=";
        GETLeaderBoardCampaignURL = API_CONNECTION + "leaderboard_campaign?user_id=";
        GETCampaignLevelURL = API_CONNECTION + "level_campaign?user_id=";
        GETLevelCheckURL = API_CONNECTION + "level_check?user_id=";


        GETLevelInfoCollocationURL = API_CONNECTION + "col/level_info?user_id=";
        GETGameInfoCollocationURL = API_CONNECTION + "col/game_info?user_id=";
        GETCollocationsCampaignLevelURL = API_CONNECTION + "col/level_campaign?user_id=";
        POSTLevelCheckInsertSoloURL = API_CONNECTION + "col/insert/level_check?user_id=";
        POSTLevelSaveScoreDragSoloURL = API_CONNECTION + "col/drag/save_score?user_id=";
        POSTLevelSaveScoreChooseSoloURL = API_CONNECTION + "col/choose/save_score?user_id=";
        POSTCollocationSetWieghtSoloURL = API_CONNECTION + "col/choose/set_weight?user_id=";
        GETCollocationLeaderBoardCampaignURL = API_CONNECTION + "col/leaderboard_campaign?user_id=";

        GETPlayModeURL = API_CONNECTION + "game/modes";

        GETLanguageJSONURL = API_CONNECTION + "language?code=";


        POSTPublishScoreURL = API_CONNECTION + "publish_score?user_id=";
        POSTRegisterForm = API_CONNECTION + "register?user_id=";
        POSTLogin = API_CONNECTION + "login";
        POSTEmailForm = API_CONNECTION + "set_email";
    }

    public static Dictionary<string, string> servers;

    // FX/music
    public static bool soundFX;
    public static bool music;

    // Networking
    // Server adresses
    //const string connectionURL = "localhost";
    //public static string port = "3000";
    //public static string connectionURL = "192.168.3.150";

    //const string connectionURL = "igre.cjvt.si";
    //const string port = "8456";

    const string connectionURL = "igrabesed.dev.mortar.tovarnaidej.com";
    const string port = "443";

    // Queries URL
    public static string API_CONNECTION = "https://" + connectionURL + ":" + port + "/api/v1/";

    // GET queries
    public static string GETConnectURL = API_CONNECTION + "connect?user_id=";
    public static string GETConnectURLSolo = API_CONNECTION + "connect?user_id=";
    public static string GETConnectImmediateURL = API_CONNECTION + "connect_immediate?user_id=";
    public static string GETConnectImmediateURLSolo = API_CONNECTION + "connect_immediate?user_id=";
    public static string GETScoreboardURL = API_CONNECTION + "scoreboard?user_id=";
    public static string GETScoreboardGlobalURL = API_CONNECTION + "global_scoreboard?user_id=";
    public static string GETScoreboardGlobalThematicURL = API_CONNECTION + "thematic_scoreboard?user_id=";

    //SYNONYM GET
    public static string GETLevelInfoURL = API_CONNECTION + "level_info?user_id=";
    public static string GETLevelScoreURL = API_CONNECTION + "level_score?user_id=";
    public static string GETLeaderBoardCampaignURL = API_CONNECTION + "leaderboard_campaign?user_id=";
    public static string GETCampaignLevelURL = API_CONNECTION + "level_campaign?user_id=";
    public static string GETLevelCheckURL = API_CONNECTION + "level_check?user_id=";

    //COLLOCATIONS GET
    public static string GETLevelInfoCollocationURL = API_CONNECTION + "col/level_info?user_id=";
    public static string GETGameInfoCollocationURL = API_CONNECTION + "col/game_info?user_id=";
    public static string GETCollocationsCampaignLevelURL = API_CONNECTION + "col/level_campaign?user_id=";
    public static string POSTLevelCheckInsertSoloURL = API_CONNECTION + "col/insert/level_check?user_id=";
    public static string POSTLevelSaveScoreDragSoloURL = API_CONNECTION + "col/drag/save_score?user_id=";
    public static string POSTLevelSaveScoreChooseSoloURL = API_CONNECTION + "col/choose/save_score?user_id=";
    public static string POSTCollocationSetWieghtSoloURL = API_CONNECTION + "col/choose/set_weight?user_id=";
    public static string GETCollocationLeaderBoardCampaignURL = API_CONNECTION + "col/leaderboard_campaign?user_id=";

    public static string GETPlayModeURL = API_CONNECTION + "game/modes";

    public static string GETLanguageJSONURL = API_CONNECTION + "language?code=";

    // POST queries
    public static string POSTPublishScoreURL = API_CONNECTION + "publish_score?user_id=";
    public static string POSTRegisterForm = API_CONNECTION + "register?user_id=";
    public static string POSTLogin = API_CONNECTION + "login";
    public static string POSTEmailForm = API_CONNECTION + "set_email";
}
