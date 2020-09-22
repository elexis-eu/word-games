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

mycursorDict.execute("SELECT id, filename FROM admin_imports WHERE type = 'col_dict' AND status = 'in_progress' LIMIT 1")
existing_import = mycursorDict.fetchone()

if existing_import is not None:
    print("Import " + existing_import['id'] + " already running!")
    exit()

mycursorDict.execute("SELECT * FROM admin_imports WHERE type = 'col_dict' AND status = 'uploaded' LIMIT 1")
new_import = mycursorDict.fetchone()

if new_import is None:
    exit()

mycursor.execute("UPDATE admin_imports SET status = 'in_progress', started = NOW() WHERE id = '" + str(new_import['id']) + "'")
mydb.commit()

try:
    mycursor.execute("SELECT * FROM structure")

    structures = mycursor.fetchall()

    structure_dict = {}
    for structure in structures: 
        structure_dict[structure[1]] = structure[0]

    # 100883|pbz0 SBZ0|2|porocen|prstan|21|6.02|0|porocni/porocan/porocun/porocn

    with open("../../"+new_import['fileondisk'], 'r', encoding='utf-8') as csvfile:
        dataReader = csv.reader(csvfile, delimiter=new_import['delimiter'])
        header = next(dataReader)
        for row in dataReader:

            mycursorDict.execute("SELECT * FROM collocation WHERE id = '" + row[0] + "'")
            existing_collocation = mycursorDict.fetchone()

            if existing_collocation is None:
                collocations.append({"id": int(row[0]), "structure": row[1], "position": row[2], "word1": row[3], "word2": row[4], "frequency": row[5], "logdice": row[6], "weight": row[7], "variants": row[8]})

                if row[2] == 2:
                    words.append({"id": int(row[0]), "word": row[3], "variants": row[8]})
                    words.append({"id": int(row[0]), "word": row[4], "variants": ""})
                else:
                    words.append({"id": int(row[0]), "word": row[3], "variants": ""})
                    words.append({"id": int(row[0]), "word": row[4], "variants": row[8]})        

    mycursor.execute("UPDATE admin_imports SET task_all = '" + str(len(collocations) + len(words)) + "' WHERE id = '" + str(new_import['id']) + "'")
    mydb.commit()

    # insert all words and save ids to dictionary
    word_dict = {}
    for word in words:

        try:
            l_unit_sql = "INSERT INTO linguistic_unit (id) values (null) "
            mycursor.execute(l_unit_sql)
            linguistic_id = mycursor.lastrowid
            #print("Linguistic_id", linguistic_id)

            s_sql = "INSERT INTO word (text, linguistic_unit_id, variants) VALUES (%s, %s, %s)"
            s_val = (word["word"], linguistic_id, word["variants"])
            mycursor.execute(s_sql, s_val)
            word_id = mycursor.lastrowid
            #print("Word_id", word_id)

            if word["id"] not in word_dict:
                word_dict[word["id"]] = {}

            word_dict[word["id"]][word["word"]] = word_id

            mycursor.execute("UPDATE admin_imports SET task_done = task_done + 1 WHERE id = '" + str(new_import['id']) + "'")
            mydb.commit()

        except Exception as e:

            s_sql = "INSERT INTO admin_report_log (import_id, error, line) VALUES (%s, %s, %s)"
            s_val = (new_import['id'], str(e), str(word))
            mycursor.execute(s_sql, s_val)
            mydb.commit()

    i = 0

    for row in collocations:

        try:
            word_sql = "INSERT INTO collocation (id, form, frequency, sailence, status, structure_id) VALUES (%s, %s, %s, %s, %s, %s)"
            word_val = (row["id"], "unknown", row["frequency"], row["logdice"].replace(",", "."), 'unknown', structure_dict[row["structure"]])
            mycursor.execute(word_sql, word_val)
            #collocation_id = mycursor.lastrowid
            collocation_id = row["id"]
            #print("Collocation_id", collocation_id)

            word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
            word_val = (word_dict[row["id"]][row["word1"]], collocation_id, 1)
            mycursor.execute(word_sql, word_val)
            collocation_word_id = mycursor.lastrowid
            #print("Collocation_word_id", collocation_word_id)

            word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
            word_val = (word_dict[row["id"]][row["word2"]], collocation_id, 2)
            mycursor.execute(word_sql, word_val)
            collocation_word_id = mycursor.lastrowid
            #print("Collocation_word_id", collocation_word_id)

            word_sql = "INSERT INTO collocation_priority (collocation_id, specific_weight, priority, total_weight, weight_limit, game_type) VALUES (%s, %s, 0, 0, 20, 1)"
            word_val = (collocation_id, row['weight'])
            mycursor.execute(word_sql, word_val)
            collocation_priority_id = mycursor.lastrowid
            #print("Collocation_priority_id", collocation_priority_id)

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
