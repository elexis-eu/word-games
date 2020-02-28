import mysql.connector
import csv

mydb = mysql.connector.connect(
    host="localhost",
    port="3306",
    user="root",
    passwd="root",
    database="igra_english"
)

csv_filename = 'dictionary_english.csv'

collocations = []
words = []
'''
#SLOVENIAN
structures = {(1, "gbz SBZ4", 2, "glagol + samostalnik v tožilniku"),
              (2, "rbz PBZ0", 2, "prislov + pridevnik"),
              (3, "RBZ gbz", 1, "prislov + glagol"),
              (4, "rbz GBZ", 2, "prislov + glagol"),
              (5, "pbz0 SBZ0", 2, "pridevnik + samostalnik"),
              (6, "GBZ sbz4", 1, "glagol + samostalnik v tožilniku"),
              (7, "PBZ0 sbz0", 1, "pridevnik + samostalnik"),
              (8, "RBZ pbz0", 1, "prislov + pridevnik"),
              (9, "SBZ0 sbz2", 1, "samostalnik + samostalnik v rodilniku")}
'''

'''
#ESTONIAN
structures = {(1, "Adj_modifier", 2, "omadussõna + nimisõna"),
              (2, "Adv_modifier", 2, "määrsõna + omadussõna"),
              (3, "modifies", 1, "omadussõna + nimisõna"),
              (4, "V_modifies", 1, "määrsõna + tegusõna"),
              (5, "modifier", 2, "määrsõna + määrsõna"),
              }
'''

'''
#DUTCH
structures = {(1, "adjective_modifier_of_\"%w\"", 2, "bijvoeglijk naamwoord + zelfstandig naamwoord"),
              (2, "adjectives_with_%w", 2, "bijvoeglijk naamwoord + werkwoord"),
              (3, "adverbs_with_%w", 2, "bijwoord + werkwoord"),
              (4, "noun_objects_with_%w", 2, "lijdend voorwerp + werkwoord"),
              (5, "noun_subjects_with_%w", 2, "onderwerp + werkwoord"),
              (6, "nouns_modified_by\"_\"%w\"", 1, "bijvoeglijk naamwoord + zelfstandig naamwoord"),
              (7, "nouns_with_%w", 2, "zelfstandig naamwoord + werkwoord"),
              (8, "verbs_with_%w", 1, "werkwoord + zelfstandig naamwoord"),
              (9, "verbs_with_%w_as_noun_object", 2, "lijdend voorwerp + werkwoord"),
              (10, "verbs_with_%w_as_noun_subject", 1, "onderwerp + werkwoord"),
              (11, "verbs_with_adjective_%w", 1, "bijvoeglijk naamwoord + werkwoord"),
              }
'''

#ENGLISH
structures = {(1, "AJ_premod", 2, "adjective + noun"),
              (2, "N_mod", 2, "noun + noun"),
              (3, "object_of", 2, "verb + noun"),
              (4, "premod_N", 1, "adjective + noun"),
              (5, "NP", 1, "verb + noun"),
              (6, "N_premod_N", 1, "noun + noun"),
              }

mycursorDict = mydb.cursor(dictionary=True)
mycursor = mydb.cursor()

structure_dict = {}
for structure in structures:

    mycursorDict.execute("SELECT * FROM structure WHERE name = '" + structure[1] + "'")
    existing_structure = mycursorDict.fetchone()

    if existing_structure is None:
        s_sql = "INSERT INTO structure (id, name, headword_position, text) VALUES (%s, %s, %s, %s)"
        s_val = (structure[0], structure[1], structure[2], structure[3])
        mycursor.execute(s_sql, s_val)
        structure_id = mycursor.lastrowid
        print("Structure_id", structure_id)
        structure_dict[structure[1]] = structure_id
    else: 
        structure_dict[structure[1]] = structure[0]

# 100883|pbz0 SBZ0|2|porocen|prstan|21|6.02|0|porocni/porocan/porocun/porocn

with open(csv_filename, 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter='|')
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
    except Exception as e:
        print(word)
        print(str(e)+"\n")

mydb.commit()


mydb.commit()
i = 0

for row in collocations:

    try:
        word_sql = "INSERT INTO collocation (id, form, frequency, sailence, status, structure_id) VALUES (%s, %s, %s, %s, %s, %s)"
        word_val = (row["id"], "unknown", row["frequency"], row["logdice"].replace(",", "."), 0, structure_dict[row["structure"]])
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

        i += 1
        if i > 1000:
            mydb.commit()
            i = 0
    except Exception as e:
        print(row)
        print(word_sql)
        print(str(e)+"\n")

mydb.commit()

print(mycursor.rowcount, "record inserted.")
