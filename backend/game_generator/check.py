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

        print()
        # print("TT", task_type_id)
        # print("Structure_id", structure_id)
        # print(theme_name)
        # print("ST", start_time)
        # print("ET", end_time)
        # print("W", words)

        for word_index, word in enumerate(words):
            word = word.strip()
            answer_array = game_generator.get_random_possible_answer_words_not_id(word, structure_id, 9)
            if len(answer_array) < 9:
                print("Structure_id", structure_id)
                print(theme_name)
                print("iztočnica: ", word, "(", len(answer_array), ")")
            # else:
            #     print("word: ", word)