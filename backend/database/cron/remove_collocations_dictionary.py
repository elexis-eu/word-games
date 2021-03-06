import mysql.connector
import csv
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

collocations = []
words = []

mycursorDict = mydb.cursor(dictionary=True)
mycursor = mydb.cursor()

mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'col_dict_remove' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'col_dict_remove' AND status = 'uploaded' LIMIT 1")
new_import = mycursorDict.fetchone()

if new_import is None:
    exit()

mycursor.execute("UPDATE admin_imports SET status = 'in_progress', started = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()

try:
    # 100883

    with open("../../"+new_import['fileondisk'], 'r', encoding='utf-8') as csvfile:
        dataReader = csv.reader(csvfile, delimiter=new_import['delimiter'])

        row_count = sum(1 for row in dataReader)

        mycursor.execute("UPDATE admin_imports SET task_all = '" + str(row_count) + "' WHERE id = '" + str(new_import['id']) + "'")
        mydb.commit()

        csvfile.seek(0)
        header = next(dataReader)

        for row in dataReader:

            mycursorDict.execute("SELECT * FROM collocation WHERE id = '" + row[0] + "'")
            existing_collocation = mycursorDict.fetchone()

            if existing_collocation is not None:
                word_sql = "DELETE FROM  collocation_word  WHERE collocation_id = %s"
                word_val = (row[0],)
                mycursor.execute(word_sql, word_val)

                word_sql = "DELETE FROM collocation_priority WHERE collocation_id = %s"
                word_val = (row[0],)
                mycursor.execute(word_sql, word_val)

                word_sql = "DELETE FROM collocation_priority WHERE collocation_id = %s"
                word_val = (row[0],)
                mycursor.execute(word_sql, word_val)

                word_sql = "DELETE FROM collocation WHERE id = %s"
                word_val = (row[0],)
                mycursor.execute(word_sql, word_val)

                mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
                mydb.commit()

except Exception as e:
    s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
    s_val = (new_import['id'], str(e), 'startup')
    mycursor.execute(s_sql, s_val)
    mydb.commit()

mycursor.execute("UPDATE admin_imports SET status = 'finish', finished = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()
