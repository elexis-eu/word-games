import mysql.connector
import csv
from decimal import *
import os
import time

from config import DBconfig, CronRoot

if CronRoot.path is not None:
    os.chdir(CronRoot.path)

mydb = mysql.connector.connect(
    host=DBconfig.host,
    port=DBconfig.port,
    user=DBconfig.user,
    passwd=DBconfig.password,
    database=DBconfig.database
)

mycursor = mydb.cursor()
mycursorDict = mydb.cursor(dictionary=True)

def get_structures():
    mycursor.execute("SELECT id, name, headword_position   \
                        FROM structure;")
    rows = mycursor.fetchall()

    assoc = {}

    for (id, name, headword_position) in rows:
        assoc[id] = {"name": name, "headword_position": headword_position}

    return assoc

mycursorDict.execute("SELECT * FROM admin_exports WHERE type = 'col_all_log' and filename IS NULL AND started IS NULL LIMIT 1")
new_export = mycursorDict.fetchone()

if new_export is None:
    exit()

mycursor.execute("UPDATE admin_exports SET started = NOW() WHERE id = '" + str(new_export['id']) + "'")
mydb.commit()

filename = str(new_export['type'])+"_"+str(new_export['admin_user_id'])+"_"+time.strftime("%Y%m%d_%H%M%S")+".csv"

try:

    structures = get_structures()

    with open("../../exports/"+filename, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)

        writer.writerow(["Level", "Game Type", "Structure", "Headword Position", "Headword", "Collocate", "CollocationID", "CollocationPriority", "Score", "Collocate2", "CollocationID2", "Collocate3", "CollocationID3", "Order Rank", "Correct Order Rank", "Variant", "Offered Collocation ID", "Headword1", "Headword2", "Distractor", "User", "Native language", "Age", "Created"])

        ### Drag
        mycursorDict.execute(" SELECT l.*, cl.headword1, cl.headword2, tt.name as game_type, IF(l.col = 0, '1', '') as distractor, u.native_language, u.age, cp.priority "  +
                             " FROM collocation_log_drag l " +
                             " INNER JOIN collocation_level cl ON cl.id_collocation_level = l.collocation_level_id " +
                             " INNER JOIN task_type tt ON tt.id = cl.game_type " +
                             " LEFT JOIN user u ON u.uid = l.user " +
                             " LEFT JOIN collocation_priority cp ON cp.collocation_id = l.col " +
                             " WHERE l.created BETWEEN '" + new_export['date_from'].strftime("%Y-%m-%d") + " 00:00' AND '" + new_export['date_to'].strftime("%Y-%m-%d") + " 23:59'")
        col_drag_log = mycursorDict.fetchall()

        for log in col_drag_log:
            print(log)
            writer.writerow([log['level'], log['game_type'], structures[log['structure_id']]['name'], structures[log['structure_id']]['headword_position'],  log['word_selected'], log['word_shown'], log['col'], log['priority'], log['score'], "", "", "", "", "", "", "", "", log['headword1'], log['headword2'], log['distractor'], log['user'], log['native_language'], log['age'], str(log['created'])])

        ### Insert
        mycursorDict.execute(" SELECT l.*, cl.headword1, cl.headword2, tt.name as game_type, u.native_language, u.age " +
                             " FROM collocation_log_insert l " +
                             " INNER JOIN collocation_level cl ON cl.id_collocation_level = l.collocation_level_id " +
                             " INNER JOIN task_type tt ON tt.id = cl.game_type " +
                             " LEFT JOIN user u ON u.uid = l.user " +
                             " WHERE l.created BETWEEN '" + new_export['date_from'].strftime("%Y-%m-%d") + " 00:00' AND '" + new_export['date_to'].strftime("%Y-%m-%d") + " 23:59'")
        col_insert_log = mycursorDict.fetchall()

        for log in col_insert_log:
            print(log)
            writer.writerow([log['level'], log['game_type'], structures[log['structure_id']]['name'], structures[log['structure_id']]['headword_position'],  log['headword'], log['word1'], log['col1'], "", log['score1'], "", "", "", "", "", "", log['variant1'], "", log['headword1'], "", "", log['user'], log['native_language'], log['age'], str(log['created'])])
            writer.writerow([log['level'], log['game_type'], structures[log['structure_id']]['name'], structures[log['structure_id']]['headword_position'],  log['headword'], log['word2'], log['col2'], "", log['score2'], "", "", "", "", "", "", log['variant2'], "", log['headword1'], "", "", log['user'], log['native_language'], log['age'], str(log['created'])])
            writer.writerow([log['level'], log['game_type'], structures[log['structure_id']]['name'], structures[log['structure_id']]['headword_position'],  log['headword'], log['word3'], log['col3'], "", log['score3'], "", "", "", "", "", "", log['variant3'], "", log['headword1'], "", "", log['user'], log['native_language'], log['age'], str(log['created'])])
        
        ### Choose
        mycursorDict.execute( " SELECT l.*, cl.headword1, cl.headword2, tt.name as game_type, (clco.choose_position+1) as order_position, cluc.choose_position, u.native_language, u.age " +
                              " FROM collocation_log_choose l " +
                              " INNER JOIN collocation_level cl ON cl.id_collocation_level = l.collocation_level_id  " +
                              " LEFT JOIN collocation_log_choose_order clco ON clco.session = l.session AND clco.collocation_id = l.col " +
                              " LEFT JOIN collocation_level_user_choose cluc ON cluc.session = l.session AND cluc.collocation_id = l.col " +
                              " INNER JOIN task_type tt ON tt.id = cl.game_type " +
                              " LEFT JOIN user u ON u.uid = l.user " +
                              " WHERE l.created BETWEEN '" + new_export['date_from'].strftime("%Y-%m-%d") + " 00:00' AND '" + new_export['date_to'].strftime("%Y-%m-%d") + " 23:59' "+
                              " ORDER BY cluc.choose_position")
        col_choose_log = mycursorDict.fetchall()

        saved_session = ""

        for log in col_choose_log:
            print(log)
            mycursorDict.execute(   " SELECT cluc2.* " +
                                    " FROM collocation_level_user_choose cluc " +
                                    " INNER JOIN collocation_level_user_choose cluc2 ON cluc2.session = cluc.session AND cluc2.group = cluc.group " +
                                    " WHERE cluc.session = '" + log['session'] + "' AND  cluc.collocation_id = " + str(log['col']) + " AND  cluc2.collocation_id != "+str(log['col']))
                                    #" WHERE cluc.collocation_level_id = " + log['collocation_level_id'] + " AND cluc.uid = '" + log['user'] + "' AND cluc.collocation_id = " + log['col'] + " AND cluc2.collocation_id != " + log['col'])
            col_choose_options = mycursorDict.fetchall()

            if(log['session'] != saved_session or log['session'] is None):
                pos_counter = 1
                saved_session = str(log['session'])
            else:
                pos_counter +=1

            writer.writerow([log['level'], log['game_type'], structures[log['structure_id']]['name'], structures[log['structure_id']]['headword_position'],  log['headword'], log['word_selected'], log['col'], "", log['score'], col_choose_options[0]['word'], col_choose_options[0]['collocation_id'], col_choose_options[1]['word'], col_choose_options[1]['collocation_id'], str(log['order_position']), str(pos_counter), "", "", log['headword1'], "", "", log['user'], log['native_language'], log['age'], str(log['created'])])
                                
        mycursor.execute("UPDATE admin_exports SET filename = '" + filename + "', finished = NOW() WHERE id = '" + str(new_export['id']) + "'")
        mydb.commit()

except Exception as e:
    print(str(e))