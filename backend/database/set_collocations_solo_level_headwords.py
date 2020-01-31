import mysql.connector
import csv
from decimal import *

mydb = mysql.connector.connect(
    host="localhost",
    port="3306",
    user="root",
    passwd="root",
    database="igra_besed"
)

mycursor = mydb.cursor()

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
    s_val = (level, position)
    mycursor.execute(s_sql, s_val)

    mydb.commit()

def insert_level_headword(level, game_type, structure_id, headword1, headword2, position):

        s_sql = "INSERT INTO collocation_level (level, game_type, structure_id, headword1, headword2, position, active) VALUES (%s, %s, %s, %s, %s, %s, 1)"
        s_val = (level, game_type, structure_id, headword1, headword2, position)
        mycursor.execute(s_sql, s_val)

        mydb.commit()

with open('collocations_level_headwords.csv', 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter=',')
    header = next(dataReader)

    structures = get_structure_ids()
    game_types = get_game_type_ids()

    #print(structures)
    #print(game_types)

    for row in dataReader:
        print(row)
        disable_previous_level(row[0], row[5])
        insert_level_headword(row[0], game_types[row[1]], structures[row[2]], row[3], row[4], row[5])

