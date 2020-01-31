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

with open('synonym_headwords_levels.csv', 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter=',')
    header = next(dataReader)

    for row in dataReader:
        print(row[0])

        if get_word_ids(row[0]) is not None:
            word_id, lu_id = get_word_ids(row[0])
            update_syn(lu_id, row[1])            
            print("Inserted")
        else:
            print("Error")
            print(row[0])

