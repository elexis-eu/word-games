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

mycursorDict.execute("SELECT * FROM admin_exports WHERE type = 'col_status_priority' AND filename IS NULL AND started IS NULL LIMIT 1")
new_export = mycursorDict.fetchone()

if new_export is None:
    exit()

mycursor.execute("UPDATE admin_exports SET started = NOW() WHERE id = '" + str(new_export['id']) + "'")
mydb.commit()

filename = str(new_export['type'])+"_"+str(new_export['admin_user_id'])+"_"+time.strftime("%Y%m%d_%H%M%S")+".csv"

try:

    with open("../../exports/"+filename, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)

        mycursorDict.execute(" SELECT c.id, c.status, p.priority, p.specific_weight, p.total_weight, p.weight_limit " +
                             " FROM collocation c " +
                             " LEFT JOIN collocation_priority p ON p.collocation_id = c.id")
        col_data = mycursorDict.fetchall()

        writer.writerow(["ID", "Status", "Priority", "Specific weight", "Total weight", "Weight limit"])

        for log in col_data:
            #print(log)
            writer.writerow([log['id'], log['status'], log['priority'], log['specific_weight'], log['total_weight'], log['weight_limit']])
        
        mycursor.execute("UPDATE admin_exports SET filename = '" + filename + "', finished = NOW() WHERE id = '" + str(new_export['id']) + "'")
        mydb.commit()

except Exception as e:
    print(str(e))