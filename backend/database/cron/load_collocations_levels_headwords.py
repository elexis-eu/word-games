import mysql.connector
import csv
from decimal import *
import os

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

def get_structure_ids():
    mycursor.execute("SELECT name, id   \
                        FROM structure;")
    rows = mycursor.fetchall()

    assoc = {}

    #columns = [col[0] for col in mycursor.description]
    #rows = [dict(zip(columns, row)) for row in mycursor.fetchall()]

    for (name, id) in rows:
        assoc[name] = id

    return assoc

def get_game_type_ids():
    mycursor.execute("SELECT name, id   \
                        FROM task_type;")
    rows = mycursor.fetchall()

    assoc = {}

    #columns = [col[0] for col in mycursor.description]
    #rows = [dict(zip(columns, row)) for row in mycursor.fetchall()]

    for (name, id) in rows:
        assoc[name] = id

    return assoc

def disable_previous_level(level, position):
    s_sql = "UPDATE collocation_level SET active = 0, deactivated = NOW() WHERE level = %s AND position = %s"
    s_val = (str(level), str(position))
    mycursor.execute(s_sql, s_val)

    mydb.commit()

def disable_whole_level(level):
    s_sql = "UPDATE collocation_level SET active = 0, deactivated = NOW() WHERE level = %s"
    s_val = (str(level),)
    mycursor.execute(s_sql, s_val)

    mydb.commit()

def insert_level_headword(level, game_type, structure_id, headword1, headword2, position, points_multiplier):

        s_sql = "INSERT INTO collocation_level (level, game_type, structure_id, headword1, headword2, position, points_multiplier, active) VALUES (%s, %s, %s, %s, %s, %s, %s, 1)"
        s_val = (level, game_type, structure_id, headword1, headword2, position, points_multiplier)
        mycursor.execute(s_sql, s_val)

        mydb.commit()


mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'col_levels_headword' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'col_levels_headword' AND status = 'uploaded' LIMIT 1")
new_import = mycursorDict.fetchone()

if new_import is None:
    exit()

mycursor.execute("UPDATE admin_imports SET status = 'in_progress', started = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()

try:
    with open("../../"+new_import['fileondisk'], 'r', encoding='utf-8') as csvfile:
        dataReader = csv.reader(csvfile, delimiter=new_import['delimiter'])
        header = next(dataReader)

        structures = get_structure_ids()
        game_types = get_game_type_ids()

        #print(structures)
        #print(game_types)

        row_count = sum(1 for row in dataReader)

        mycursor.execute("UPDATE admin_imports SET task_all = '" + str(row_count) + "' WHERE id = '" + str(new_import['id']) + "'")
        mydb.commit()

        csvfile.seek(0)
        header = next(dataReader)

        for row in dataReader:
            try:
                if len(row) == 1:
                    disable_whole_level(row[0])
                elif len(row) == 2:
                    if row[1] == '':
                        disable_whole_level(row[0])
                    else:
                        disable_previous_level(row[0], row[1])
                else:
                    disable_previous_level(row[0], row[5])
                    insert_level_headword(row[0], game_types[row[1]], structures[row[2]], row[3], row[4], row[5], row[6])

                mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
                mydb.commit()

            except Exception as e:
                s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
                s_val = (new_import['id'], str(e), str(row))
                mycursor.execute(s_sql, s_val)
                mydb.commit()

                print(row)
                print(str(e))
                
except Exception as e:
    s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
    s_val = (new_import['id'], str(e), 'startup')
    mycursor.execute(s_sql, s_val)
    mydb.commit()

    print(str(e))

mycursor.execute("UPDATE admin_imports SET status = 'finish', finished = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()
