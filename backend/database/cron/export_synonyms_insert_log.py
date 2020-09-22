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

mycursorDict.execute("SELECT * FROM admin_exports WHERE type = 'syn_insert_log' and filename IS NULL AND started IS NULL LIMIT 1")
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

        mycursorDict.execute("SELECT * FROM synonym_log WHERE created BETWEEN '" + new_export['date_from'].strftime("%Y-%m-%d") + " 00:00' AND '" + new_export['date_to'].strftime("%Y-%m-%d") + " 23:59'")
        col_log = mycursorDict.fetchall()

        writer.writerow(["Level", "Headword", "Word1", "Score1", "Word2", "Score2", "Word3", "Score3", "User", "Created"])

        for log in col_log:
            print(log)
            writer.writerow([log['level'], log['headword'], log['word1'], log['score1'], log['word2'], log['score2'], log['word3'], log['score3'], log['user'], str(log['created'])])
        
        mycursor.execute("UPDATE admin_exports SET filename = '" + filename + "', finished = NOW() WHERE id = '" + str(new_export['id']) + "'")
        mydb.commit()

except Exception as e:
    print(str(e))