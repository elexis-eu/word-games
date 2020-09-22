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
                        INNER JOIN linguistic_unit as lu ON lu.id = w.linguistic_unit_id WHERE w.text = BINARY \"{}\" LIMIT 1 ;".format(word))
    r = mycursor.fetchone()
    return r

def add_word(headword):
        l_unit_sql = "INSERT INTO linguistic_unit (id) values (null) "
        mycursor.execute(l_unit_sql)
        linguistic_id = mycursor.lastrowid
        #print("Linguistic_id", linguistic_id)

        #lexeme_sql = "INSERT IGNORE INTO lexeme (id) values (%s) "
        #s_val = (headword_id,)
        #print("lexeme_id", s_val)
        #mycursor.execute(lexeme_sql, s_val)

        #s_sql = "INSERT INTO word (text, linguistic_unit_id, lexeme_id) VALUES (%s, %s, %s)"
        #s_val = (headword, linguistic_id, headword_id)
        s_sql = "INSERT INTO word (text, linguistic_unit_id) VALUES (%s, %s)"
        s_val = (headword, linguistic_id)
        mycursor.execute(s_sql, s_val)
        word_id = mycursor.lastrowid
        #print("Word_id", word_id)
        
        mydb.commit()

        return (word_id, linguistic_id)

def add_syn(headword_id, syn_id, score, stype):

        s_sql = "REPLACE INTO  synonym (linguistic_unit_id, linguistic_unit_id_syn, score, type) VALUES (%s, %s, %s, %s)"
        s_val = (headword_id, syn_id, score, stype)
        mycursor.execute(s_sql, s_val)

        mydb.commit()

mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'synonyms_dict' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'synonyms_dict' AND status = 'uploaded' LIMIT 1")
new_import = mycursorDict.fetchone()

if new_import is None:
    exit()

mycursor.execute("UPDATE admin_imports SET status = 'in_progress', started = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()

try:

    import xml.etree.ElementTree as ET
    tree = ET.parse("../../"+new_import['fileondisk'])
    root = tree.getroot()

    all_entries = root.findall('entry')

    mycursor.execute("UPDATE admin_imports SET task_all = '" + str(len(all_entries)) + "' WHERE id = '" + str(new_import['id']) + "'")
    mydb.commit()

    for entry in all_entries:
        try:
            headword = entry.find("headword").text
            headword_id = entry.find("headword").get("id")

            print(headword)

            findword = get_word_ids(headword)

            if findword is None:
                headword_id, headling_id = add_word(headword)
            else:
                headword_id = findword[0]
                headling_id = findword[1]

            if entry.find("groups_core") is not None:
                for group_core in entry.find("groups_core").findall("group"):
                    for candidate in group_core.findall("candidate"):
                        cand_word = candidate.find("s").text
                        cand_score = candidate.get("score")

                        findword = get_word_ids(cand_word)

                        if findword is None:
                            candword_id, candling_id = add_word(cand_word)
                        else:
                            candword_id = findword[0]
                            candling_id = findword[1]

                        add_syn(headling_id, candling_id, round(Decimal(cand_score)*100, 0), 'core')

            if entry.find("groups_near") is not None:
                for group_near in entry.find("groups_near").findall("group"):
                    for candidate in group_near.findall("candidate"):
                        cand_word = candidate.find("s").text
                        cand_score = candidate.get("score")

                        findword = get_word_ids(cand_word)

                        if findword is None:
                            candword_id, candling_id = add_word(cand_word)
                        else:
                            candword_id = findword[0]
                            candling_id = findword[1]

                        add_syn(headling_id, candling_id, round(Decimal(cand_score)*100, 0), 'near')
            
            mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
            mydb.commit()
        except Exception as e:
            s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
            s_val = (new_import['id'], str(e), str(entry))
            mycursor.execute(s_sql, s_val)
            mydb.commit()

except Exception as e:
    s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
    s_val = (new_import['id'], str(e), 'startup')
    mycursor.execute(s_sql, s_val)
    mydb.commit()

mycursor.execute("UPDATE admin_imports SET status = 'finish', finished = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()