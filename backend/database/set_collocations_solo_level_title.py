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

def update_level_title(level, title, next_round):

        s_sql = "REPLACE INTO collocation_level_title (level, title, next_round) VALUES (%s, %s, %s)"
        s_val = (level, title, next_round)
        mycursor.execute(s_sql, s_val)

        mydb.commit()

with open('collocations_level_title.csv', 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter=',')
    header = next(dataReader)

    for row in dataReader:
        print(row[0])
        update_level_title(row[0], row[1], row[2])

