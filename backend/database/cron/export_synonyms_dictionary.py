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

mycursorDict.execute("SELECT * FROM admin_exports WHERE type = 'synonyms_dict' AND filename IS NULL AND started IS NULL LIMIT 1")
new_export = mycursorDict.fetchone()

if new_export is None:
    exit()

mycursor.execute("UPDATE admin_exports SET started = NOW() WHERE id = '" + str(new_export['id']) + "'")
mydb.commit()

filename = str(new_export['type'])+"_"+str(new_export['admin_user_id'])+"_"+time.strftime("%Y%m%d_%H%M%S")+".csv"

try:

    with open("../../exports/"+filename, 'w', newline='', encoding='utf-8') as csvfile:

        mycursorDict.execute("SELECT max(linguistic_unit_id) as max, max(linguistic_unit_id_syn) as max2 FROM synonym")
        col_max = mycursorDict.fetchone()

        writer = csv.writer(csvfile)

        mycursorDict.execute(   " SELECT s.linguistic_unit_id, s.linguistic_unit_id_syn, sw.text as word1, sw2.text as word2, s.tid, s.difficulty as level, s.type, s.score " +
                                " FROM synonym s " +
                                " INNER JOIN synonym_word sw ON sw.linguistic_unit_id = s.linguistic_unit_id " +
                                " INNER JOIN synonym_word sw2 ON sw2.linguistic_unit_id = s.linguistic_unit_id_syn " )
        #col_log = mycursorDict.fetchall()

        writer.writerow(["Word1", "Word2", "TID", "Level", "Type", "Score"])

        while True:
            col_data = mycursorDict.fetchmany(10000)
            
            print(len(col_data))

            for row in col_data:
                try:
                    writer.writerow([row['word1'], row['word2'], row['tid'], row['level'], row['type'], row['score']])
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