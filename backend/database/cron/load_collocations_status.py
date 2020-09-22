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

def update_status(collocation_id, status):

        s_sql = "UPDATE collocation SET status = '" + status + "' WHERE id = " + collocation_id
        mycursor.execute(s_sql)

        mydb.commit()

mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'col_status' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'col_status' AND status = 'uploaded' LIMIT 1")
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
                print(row[0])
                update_status(row[0], row[1])

                mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
                mydb.commit()
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