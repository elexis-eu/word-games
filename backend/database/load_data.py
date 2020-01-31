import mysql.connector
import csv

mydb = mysql.connector.connect(
    host="localhost",
    port="3305",
    user="root",
    passwd="root",
    database="igra_besed"
)

csv_filename = 'igra_besed_948-v4.csv'

collocations = []
words = set()
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
    s_sql = "INSERT INTO structure (id, name, headword_position, text) VALUES (%s, %s, %s, %s)"
    s_val = (structure[0], structure[1], structure[2], structure[3])
    mycursor.execute(s_sql, s_val)
    structure_id = mycursor.lastrowid
    print("Structure_id", structure_id)
    structure_dict[structure[1]] = structure_id


with open(csv_filename, 'r', encoding='utf-8') as csvfile:
    dataReader = csv.reader(csvfile, delimiter='|')
    header = next(dataReader)
    for row in dataReader:
        # print(row)
        if not row[7].replace('.', '', 1).isdigit():
            row[7] = 0
            row[8] = 0.0
        collocations.append((int(row[0]), row[1], int(row[2]), int(row[3]), row[4],
                             int(row[5]), row[6], int(row[7]), float(row[8])))
        # structures.add((row[1], row[2]))
        words.add((int(row[3]), row[4], row[9]))
        words.add((int(row[5]), row[6]))

structure_sql = "INSERT INTO structure (name) VALUES (%s)"
# Adjective = ("Adjective",)
# Noun = ("Noun",)
#
# mycursor.execute(structure_sql, Adjective)
# adjective_id = mycursor.lastrowid
# print("Adjective id", adjective_id)
#
# mycursor.execute(structure_sql, Noun)
# noun_id = mycursor.lastrowid
# print("Noun id", noun_id)

# insert all words and save ids to dictionary
word_dict = {}
for word in words:
    l_unit_sql = "INSERT INTO linguistic_unit (id) values (null) "
    mycursor.execute(l_unit_sql)
    linguistic_id = mycursor.lastrowid
    print("Linguistic_id", linguistic_id)

    lexeme_sql = "INSERT IGNORE INTO lexeme (id) values (%s) "
    s_val = (word[0],)
    print("lexeme_id", s_val)
    mycursor.execute(lexeme_sql, s_val)

    s_sql = "INSERT INTO word (text, linguistic_unit_id, lexeme_id, variants) VALUES (%s, %s, %s, %s)"
    s_val = (word[1], linguistic_id, word[0], word[2])
    mycursor.execute(s_sql, s_val)
    word_id = mycursor.lastrowid
    print("Word_id", word_id)
    word_dict[word[1]] = word_id

mydb.commit()


mydb.commit()
i = 0

for row in collocations:
    c_sql = "INSERT INTO collocation (id, form, frequency, sailence, status, structure_id) VALUES (%s, %s, %s, %s, %s, %s)"
    c_val = (row[0], "unknown", row[7], row[8], 0, structure_dict[row[1]])
    mycursor.execute(c_sql, c_val)
    collocation_id = mycursor.lastrowid
    print("Collocation_id", collocation_id)

    word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
    word_val = (word_dict[row[4]], collocation_id, 1)
    mycursor.execute(word_sql, word_val)
    collocation_word_id = mycursor.lastrowid
    print("Collocation_word_id", collocation_word_id)

    word_sql = "INSERT INTO collocation_word (word_id, collocation_id, position) VALUES (%s, %s, %s)"
    word_val = (word_dict[row[6]], collocation_id, 2)
    mycursor.execute(word_sql, word_val)
    collocation_word_id = mycursor.lastrowid
    print("Collocation_word_id", collocation_word_id)

    i += 1
    if i > 1000:
        mydb.commit()
        i = 0

mydb.commit()

print(mycursor.rowcount, "record inserted.")
