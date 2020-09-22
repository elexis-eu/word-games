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

def get_word_ids(word):
    mycursor.execute("SELECT w.id as word_id, lu.id as lu_id  \
                        FROM word as w \
                        INNER JOIN linguistic_unit as lu ON lu.id = w.linguistic_unit_id WHERE BINARY w.text = \"{}\" LIMIT 1 ;".format(word))
    r = mycursor.fetchone()
    return r

def update_syn(headword_id, difficulty):

        s_sql = "UPDATE synonym SET difficulty = %s WHERE linguistic_unit_id =  %s"
        s_val = (difficulty, headword_id)
        mycursor.execute(s_sql, s_val)

        mydb.commit()

mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'synonyms_levels' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'synonyms_levels' AND status = 'uploaded' LIMIT 1")
new_import = mycursorDict.fetchone()

if new_import is None:
    exit()

mycursor.execute("UPDATE admin_imports SET status = 'in_progress', started = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()

try:
    with open("../../"+new_import['fileondisk'], 'r', encoding='utf-8') as csvfile:
        dataReader = csv.reader(csvfile, delimiter=new_import['delimiter'])
        header = next(dataReader)

        row_count = sum(1 for row in dataReader)

        mycursor.execute("UPDATE admin_imports SET task_all = '" + str(row_count) + "' WHERE id = '" + str(new_import['id']) + "'")
        mydb.commit()

        csvfile.seek(0)
        header = next(dataReader)

        for row in dataReader:

            try:
                if get_word_ids(row[0]) is not None:
                    word_id, lu_id = get_word_ids(row[0])
                    update_syn(lu_id, row[1])

                    mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
                    mydb.commit()
                else:
                    raise Exception("Word not in dictionary")

            except Exception as e:
                s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
                s_val = (new_import['id'], str(e), str(row))
                mycursor.execute(s_sql, s_val)
                mydb.commit()

except Exception as e:
    s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
    s_val = (new_import['id'], str(e), 'startup')
    mycursor.execute(s_sql, s_val)
    mydb.commit()

mycursor.execute("UPDATE admin_imports SET status = 'finish', finished = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()
