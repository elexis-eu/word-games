import mysql.connector
import os
from datetime import datetime
import game_generator

THEMATIC_TASK_TYPE = 4

datetime_format = '%d.%m.%Y %H:%M'
is_new_line = True

with open('thematic_NL_propr.txt', encoding="utf8") as f:
    while is_new_line:
        task_type_id = int(f.readline())
        structure_id = int(f.readline())
        theme_name = f.readline().strip()
        start_time = datetime.strptime(f.readline().strip(), datetime_format)
        end_time = datetime.strptime(f.readline().strip(), datetime_format)
        words = f.readline().strip().split(',')
        if not f.readline():
            is_new_line = False

        print("TT", task_type_id)
        print("S", structure_id)
        print("TN", theme_name)
        print("ST", start_time)
        print("ET", end_time)
        print("W", words)

        # get headword position form structure id
        game_generator.mycursor.execute("SELECT s.headword_position  \
                                FROM structure as s             \
                                WHERE s.id ='{}';".format(structure_id))
        r = game_generator.mycursor.fetchall()
        headword_position = r[0][0]
        print("headword", headword_position)

        # insert new thematic row
        game_generator.mycursor.execute("INSERT INTO thematic (name, task_type_id) values ('{}', {}) ".format(theme_name, task_type_id))
        thematic_id = game_generator.mycursor.lastrowid
        print("new insertion:", thematic_id)

        # insert new task cycle
        game_generator.mycursor.execute("INSERT INTO task_cycle (task_type_id, from_timestamp, to_timestamp, thematic_id) "
                         "values ({}, '{}', '{}', '{}') ;"
                         .format(THEMATIC_TASK_TYPE, start_time, end_time, thematic_id))
        task_cycle_id = game_generator.mycursor.lastrowid

        # generate all cycles
        word_index = 0
        for word in words:
            word = word.strip()
            answer_array = game_generator.get_random_possible_answer_words_not_id(word, structure_id, 9)
            if len(answer_array) < 9:
                print("word: ", word, "(", len(answer_array), ") pass")
                continue

            game_generator.mycursor.execute("SELECT * FROM word as w where w.text='{}';".format(word))
            myresult = game_generator.mycursor.fetchall()
            word_id = myresult[0][0]
            for result in myresult:
                # print("Result: ", result)
                if result[1] == word:
                    word_id = result[0]
                    break
            print("word: ", word_index, word)

            if task_type_id == 1:       # Choose game type
                game_generator.generate_choose_word(structure_id, headword_position, task_cycle_id, word_id, word_index, False, word)
            elif task_type_id == 2:    # Insert game type
                game_generator.generate_insert_word(structure_id, headword_position, task_cycle_id, word_id, word_index)
            # elif task_cycle_id == 3:    # Drag game type
            #     game_generator.generate_drag_word(structure_id, headword_position, task_cycle_id, word_id, word_index)
            word_index += 1
        game_generator.sql_commit()
