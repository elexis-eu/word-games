import mysql.connector
import csv

mydb = mysql.connector.connect(
    host="localhost",
    port="3306",
    user="root",
    passwd="root",
    database="igra_estonia"
)

csv_filename = 'test_new.csv'

collocations = []
words = []
structures = {(1, "gbz SBZ4", 2, "glagol + samostalnik v tožilniku"),
              (2, "rbz PBZ0", 2, "prislov + pridevnik"),
              (3, "RBZ gbz", 1, "prislov + glagol"),
              (4, "rbz GBZ", 2, "prislov + glagol"),
              (5, "pbz0 SBZ0", 2, "pridevnik + samostalnik"),
              (6, "GBZ sbz4", 1, "glagol + samostalnik v tožilniku"),
              (7, "PBZ0 sbz0", 1, "pridevnik + samostalnik"),
              (8, "RBZ pbz0", 1, "prislov + pridevnik"),
              (9, "SBZ0 sbz2", 1, "samostalnik + samostalnik v rodilniku")}


mycursor = mydb.cursor()

structure_dict = {}
for structure in structures:
    s_sql = "REPLACE INTO structure (id, name, headword_position, text) VALUES (%s, %s, %s, %s)"
    s_val = (structure[0], structure[1], structure[2], structure[3])
    mycursor.execute(s_sql, s_val)
    structure_id = mycursor.lastrowid
    print("Structure_id", structure_id)
    structure_dict[structure[1]] = structure_id

# 100883|pbz0 SBZ0|2|porocen|prstan|21|6.02|0|porocni/porocan/porocun/porocn

with open(csv_filename, 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter='|')
    header = next(dataReader)
    for row in dataReader:
        
        collocations.append({"id": int(row[0]), "structure": row[1], "position": row[2], "word1": row[3], "word2": row[4], "frequency": row[5], "logdice": row[6], "weight": row[7], "variants": row[8]})

        if row[2] == 2:
            words.append({"word": row[3], "variants": row[8]})
            words.append({"word": row[4], "variants": ""})
        else:
            words.append({"word": row[3], "variants": ""})
            words.append({"word": row[4], "variants": row[8]})

# insert all words and save ids to dictionary
word_dict = {}
for word in words:
    l_unit_sql = "INSERT INTO linguistic_unit (id) values (null) "
    mycursor.execute(l_unit_sql)
    linguistic_id = mycursor.lastrowid
    print("Linguistic_id", linguistic_id)

    s_sql = "INSERT INTO word (text, linguistic_unit_id, variants) VALUES (%s, %s, %s)"
    s_val = (word["word"], linguistic_id, word["variants"])
    mycursor.execute(s_sql, s_val)
    word_id = mycursor.lastrowid
    print("Word_id", word_id)
    word_dict[word["word"]] = word_id

mydb.commit()


mydb.commit()
i = 0

for row in collocations:
    c_sql = "INSERT INTO collocation (id, form, frequency, sailence, status, structure_id) VALUES (%s, %s, %s, %s, %s, %s)"
    c_val = (row["id"], "unknown", row["frequency"], row["logdice"], 0, structure_dict[row["structure"]])
    mycursor.execute(c_sql, c_val)
    collocation_id = mycursor.lastrowid
    print("Collocation_id", collocation_id)

    word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
    word_val = (word_dict[row["word1"]], collocation_id, 1)
    mycursor.execute(word_sql, word_val)
    collocation_word_id = mycursor.lastrowid
    print("Collocation_word_id", collocation_word_id)

    word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
    word_val = (word_dict[row["word2"]], collocation_id, 2)
    mycursor.execute(word_sql, word_val)
    collocation_word_id = mycursor.lastrowid
    print("Collocation_word_id", collocation_word_id)

    word_sql = "INSERT INTO collocation_priority (collocation_id, specific_weight, priority, total_weight, weight_limit, game_type) VALUES (%s, %s, 0, 0, 20, 1)"
    word_val = (collocation_id, row['weight'])
    mycursor.execute(word_sql, word_val)
    collocation_priority_id = mycursor.lastrowid
    print("Collocation_priority_id", collocation_priority_id)

    i += 1
    if i > 1000:
        mydb.commit()
        i = 0

mydb.commit()

print(mycursor.rowcount, "record inserted.")
