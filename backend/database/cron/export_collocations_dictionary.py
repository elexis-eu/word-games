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

mycursorDict.execute("SELECT * FROM admin_exports WHERE type = 'col_dict' AND filename IS NULL AND started IS NULL LIMIT 1")
new_export = mycursorDict.fetchone()

if new_export is None:
    exit()

mycursor.execute("UPDATE admin_exports SET started = NOW() WHERE id = '" + str(new_export['id']) + "'")
mydb.commit()

filename = str(new_export['type'])+"_"+str(new_export['admin_user_id'])+"_"+time.strftime("%Y%m%d_%H%M%S")+".csv"

try:

    structures = get_structures()

    with open("../../exports/"+filename, 'w', newline='', encoding='utf-8') as csvfile:

        mycursorDict.execute("SELECT max(id) as max FROM collocation")
        col_max = mycursorDict.fetchone()

        writer = csv.writer(csvfile)

        mycursorDict.execute(   " SELECT c.id, s.name as structure_name, s.headword_position, w1.text as wordtext1, w2.text as wordtext2, c.frequency, c.sailence, w1.variants as variants1, w2.variants as variants2 " +
                                " FROM collocation c " +
                                " INNER JOIN structure s ON c.structure_id = s.id " +
                                " INNER JOIN collocation_word word1 ON word1.collocation_id = c.id AND word1.position = 1 " +
                                " INNER JOIN word w1 ON w1.id = word1.word_id " +
                                " INNER JOIN collocation_word word2 ON word2.collocation_id = c.id AND word2.position = 2 " +
                                " INNER JOIN word w2 ON w2.id = word2.word_id ")
        #col_log = mycursorDict.fetchall()

        writer.writerow(["CollocationID", "Structure", "Headword position", "Word1", "Word2", "Frequency", "logDice", "Variants"])

        while True:
            col_data = mycursorDict.fetchmany(10000)
            print(len(col_data))

            for row in col_data:

                if row['id'] is None:
                    continue

                try:
                    variants = u""

                    if row['variants1'] is not None:
                        variants = row['variants1']

                    if row['variants2'] is not None:
                        variants = row['variants2']

                    writer.writerow([row['id'], row['structure_name'], row['headword_position'], row['wordtext1'], row['wordtext2'], row['frequency'], row['sailence'], variants])
                except Exception as e:
                    print(str(row))
                    print(str(e))
                
            if len(col_data) == 1:
                break

            if len(col_data) == 0:
                break
            
        mycursor.execute("UPDATE admin_exports SET filename = '" + filename + "', finished = NOW() WHERE id = '" + str(new_export['id']) + "'")
        mydb.commit()

except Exception as e:
    print(str(e))