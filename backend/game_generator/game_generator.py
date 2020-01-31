import math
import mysql.connector
import datetime
import time
import os

import random
import schedule
import json
from pprint import pprint
import collections

mydb = mysql.connector.connect(
    host=os.getenv('IGRA_BESED_DATABASE_HOST', "localhost"),
    port=os.getenv('IGRA_BESED_DATABASE_PORT', "3306"),
    user=os.getenv('IGRA_BESED_DATABASE_USER', "igrabesed"),
    passwd=os.getenv('IGRA_BESED_DATABASE_PASSWD', "Igrapass!"),
    database=os.getenv('IGRA_BESED_DATABASE_SCHEMA', "igra_besed"),
)

mycursor = mydb.cursor()

game_config_file =os.getenv('IGRA_BESED_GAME_CONFIG', 'GameConfig.json'),

with open('../'+game_config_file) as f:
    game_config = json.load(f)

game_config["CHOOSE_ALL_ROUNDS_DURATION"] = (game_config["CHOOSE_DURATION_OF_ROUND"] + game_config["ROUND_PAUSE_DURATION"]) * game_config["NUMBER_OF_WORDS_IN_ONE_CYCLE"] - game_config["ROUND_PAUSE_DURATION"]
game_config["CHOOSE_DURATION_WHOLE_CYCLE"] = game_config["CHOOSE_ALL_ROUNDS_DURATION"] + game_config["DURATION_WAIT_FOR_SCORES"] + game_config["DURATION_SHOW_SCORES"]

game_config["INSERT_ALL_ROUNDS_DURATION"] = (game_config["INSERT_DURATION_OF_ROUND"] + game_config["INSERT_ROUND_PAUSE_DURATION"]) * game_config["INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE"] - game_config["INSERT_ROUND_PAUSE_DURATION"]
game_config["INSERT_DURATION_WHOLE_CYCLE"] = game_config["INSERT_ALL_ROUNDS_DURATION"] + game_config["DURATION_WAIT_FOR_SCORES"] + game_config["DURATION_SHOW_SCORES"]

game_config["SYNONYM_ALL_ROUNDS_DURATION"] = (game_config["SYNONYM_DURATION_OF_ROUND"] + game_config["SYNONYM_ROUND_PAUSE_DURATION"]) * game_config["SYNONYM_NUMBER_OF_WORDS_IN_ONE_CYCLE"] - game_config["SYNONYM_ROUND_PAUSE_DURATION"]
game_config["SYNONYM_DURATION_WHOLE_CYCLE"] = game_config["SYNONYM_ALL_ROUNDS_DURATION"] + game_config["DURATION_WAIT_FOR_SCORES"] + game_config["DURATION_SHOW_SCORES"]

game_config["DRAG_ROUNDS_DURATION"] = game_config["DRAG_DURATION_OF_ROUND"]
game_config["DRAG_DURATION_WHOLE_CYCLE"] = game_config["DRAG_ROUNDS_DURATION"] + game_config["DURATION_WAIT_FOR_SCORES"] + game_config["DURATION_SHOW_SCORES"]

pprint(game_config)
TIMESTAMP_FORMAT = '%Y-%m-%d %H:%M:%S.%f'
SCORES = [100, 85, 70, 60, 55, 50, 45, 40, 37, 35, 33, 31, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20]
CHOOSE_SCORES = game_config["CHOOSE_SCORING"]
DRAG_SCORES = game_config["DRAG_SCORING"]

EMPTY_WORD = ""
EMPTY_STRUCTURE = ("Empty",)


def sql_commit():
    mydb.commit()


def add_if_not_already(task_type_name):
    mycursor.execute("SELECT tt.id FROM task_type as tt where tt.name='{}'".format(task_type_name))
    myresult = mycursor.fetchall()

    if not myresult:
        mycursor.execute("INSERT INTO task_type (name) values ('{}') ".format(task_type_name))
        print("new insertion:", mycursor.lastrowid)
        return mycursor.lastrowid
    return myresult[0][0]


def insert_empty_word():
    l_unit_sql = "INSERT INTO linguistic_unit (id) values (null) "
    mycursor.execute(l_unit_sql)
    linguistic_id = mycursor.lastrowid
    print("New insertion: Linguistic_id", linguistic_id)

    s_sql = "INSERT INTO word (text, linguistic_unit_id) VALUES (%s, %s)"
    s_val = (EMPTY_WORD, linguistic_id)
    mycursor.execute(s_sql, s_val)
    word_id = mycursor.lastrowid
    print("New insertion: Word_id", word_id)
    return word_id


def insert_empty_structure():
    structure_sql = "INSERT INTO structure (name) VALUES (%s)"
    mycursor.execute(structure_sql, EMPTY_STRUCTURE)
    struct_id = mycursor.lastrowid
    # print("Empty Structure id", struct_id)
    return struct_id


def get_empty_word_id():
    mycursor.execute("SELECT w.linguistic_unit_id from word as w where w.text='{}'".format(EMPTY_WORD))
    myresult = mycursor.fetchall()

    if not myresult:
        word_id = insert_empty_word()
        # print("Generated: ", word_id)
    else:
        word_id = myresult[0][0]

    # print("ES: ", empty_structure[0])
    # mycursor.execute("SELECT s.id from structure as s where s.name='{}'".format(EMPTY_STRUCTURE[0]))
    # result = mycursor.fetchall()
    #
    # if not result:
    #     struc_id = insert_empty_structure()
    #     # print("Generated: ", struc_id)
    # else:
    #     struc_id = result[0][0]

    # print("Empty ids: ", word_id, struc_id)
    return word_id  # , struc_id


def get_proper_cycle_time(task_type_id):
    mycursor.execute("SELECT * FROM task_cycle as tc where tc.task_type_id={} ORDER BY id DESC LIMIT 1"
                     .format(task_type_id))
    myresult = mycursor.fetchall()
    # print("last cycle time:", myresult, " task type id: ", task_type_id)

    current_time = datetime.datetime.utcnow()
    if myresult and myresult[0][3] > current_time:
        return myresult[0][3]
    return current_time


def generate_new_task_cycle(task_type_id, from_timestamp, to_timestamp):
    mycursor.execute("INSERT INTO task_cycle (task_type_id, from_timestamp, to_timestamp) values ({}, '{}', '{}') "
                     .format(task_type_id, from_timestamp, to_timestamp))
    return mycursor.lastrowid


structures = {}
random_headword_buffer = {}
MIN_ARRAY_LENGTH = 30


def init_random_headword_buffer():
    all_structures = get_all_structures()
    for structure in all_structures:
        s_id, headword_pos = structure
        structures[s_id] = headword_pos
        fill_random_headword_buffer(s_id, headword_pos)


def get_random_headword_from_buff(structure_id, num):
    result = []
    for i in range(num):
        result.append(random_headword_buffer[structure_id].popleft())

    if len(random_headword_buffer[structure_id]) < MIN_ARRAY_LENGTH:
        fill_random_headword_buffer(structure_id, structures[structure_id])
    return result


def fill_random_headword_buffer(s_id, headword_pos):
    word_ids = get_random_headword(headword_pos, s_id, 1000)
    # print(word_ids)
    random_headword_buffer[s_id] = collections.deque()
    for w_id in word_ids:
        w_id = w_id[0]
        random_headword_buffer[s_id].append(w_id)


def get_all_structures():
    mycursor.execute("SELECT s.id, s.headword_position  \
                        FROM structure as s;")
    r = mycursor.fetchall()
    return r


def get_random_structure():
    mycursor.execute("SELECT s.id, s.headword_position  \
                        FROM structure as s             \
                        WHERE s.name != 'synonym'       \
                        ORDER BY RAND() LIMIT 1;")
    r = mycursor.fetchall()
    return r[0]


def get_random_headword(pos, structure_id, num):
    mycursor.execute("SELECT cw.word_id     \
                        FROM collocation_word as cw                         \
                        JOIN collocation as c                               \
                        ON c.id = cw.collocation_id                         \
                        WHERE cw.position = {} AND c.structure_id = {}      \
                        GROUP BY cw.word_id                                 \
                        HAVING COUNT(cw.word_id) > 8                        \
                        ORDER BY RAND() LIMIT {};".format(pos, structure_id, num))
    r = mycursor.fetchall()
    # print("grh: ", r)
    return r

def get_random_headword_synonym(min_synonynms, num, difficulty=1):
    mycursor.execute("  SELECT w.id as word_id                                                       \
                        FROM synonym s                                                    \
                        INNER JOIN word w ON w.linguistic_unit_id = s.linguistic_unit_id  \
                        WHERE s.difficulty = {}                                                                      \
                        GROUP BY s.linguistic_unit_id                                                \
                        HAVING count(distinct s.linguistic_unit_id_syn) >= {} ORDER BY RAND() LIMIT {};".format(difficulty, min_synonynms, num))
    r = mycursor.fetchall()
    # print("grh: ", r)
    return r

def insert_collocation_shell(structure_id):
    mycursor.execute("INSERT INTO collocation_shell (structure_id) VALUES ({}); "
                     .format(structure_id))
    return mycursor.lastrowid


def insert_collocation_shell_word(cs_id, w_id, position):
    mycursor.execute("INSERT INTO collocation_shell_word (collocation_shell_id, word_id, position) \
                        VALUES ({},{},{}) ".format(cs_id, w_id, position))
    return mycursor.lastrowid


def insert_task(cycle_id, collocation_shell_id, pos):
    mycursor.execute("INSERT INTO task (task_cycle_id, collocation_shell_id, position) VALUES ({}, {}, {}); "
                     .format(cycle_id, collocation_shell_id, pos))
    return mycursor.lastrowid


def get_random_possible_answer_words(word_id, structure_id, num, get_word_id=False):
    if get_word_id:
        id = "id"
    else:
        id = "linguistic_unit_id"

    mycursor.execute("SELECT DISTINCT w." + id + ", c.frequency, cw.collocation_id, c.order_value  \
                        FROM collocation_word as cw                                 \
                        JOIN word as w                                              \
                        JOIN collocation as c                                       \
                        JOIN structure as s                                         \
                        ON cw.collocation_id=c.id AND w.id=cw.word_id AND s.id = c.structure_id         \
                        WHERE cw.position!=s.headword_position AND s.id={}              \
                                AND c.id IN (SELECT cw.collocation_id           \
                                    FROM collocation_word as cw                                 \
                                    JOIN word as w                                              \
                                    ON w.id=cw.word_id                                          \
                                    WHERE w.id='{}')                                            \
                        ORDER BY RAND() LIMIT {};".format(structure_id, word_id, num))
    r = mycursor.fetchall()
    return r


def get_random_possible_answer_words_not_id(word, structure_id, num, get_word_id=False):
    if get_word_id:
        id = "id"
    else:
        id = "linguistic_unit_id"

    mycursor.execute("SELECT DISTINCT w." + id + ", c.frequency, cw.collocation_id, c.order_value  \
                        FROM collocation_word as cw                                 \
                        JOIN word as w                                              \
                        JOIN collocation as c                                       \
                        JOIN structure as s                                         \
                        ON cw.collocation_id=c.id AND w.id=cw.word_id AND s.id = c.structure_id         \
                        WHERE cw.position!=s.headword_position AND s.id={}              \
                                AND c.id IN (SELECT cw.collocation_id           \
                                    FROM collocation_word as cw                                 \
                                    JOIN word as w                                              \
                                    ON w.id=cw.word_id                                          \
                                    WHERE w.text='{}')                                            \
                        ORDER BY RAND() LIMIT {};".format(structure_id, word, num))
    r = mycursor.fetchall()
    return r


def get_random_words(num):
    mycursor.execute("SELECT DISTINCT w.id  \
                        FROM word as w                      \
                        ORDER BY RAND() LIMIT {};".format(num))
    r = mycursor.fetchall()
    return r


def get_linguistic_id(word_id):
    mycursor.execute("SELECT w.linguistic_unit_id      \
                                FROM word as w          \
                                WHERE w.id={}".format(word_id))
    r = mycursor.fetchall()
    return r[0][0]


def insert_possible_answer(linguistic_unit_id):
    mycursor.execute("INSERT INTO possible_answer (linguistic_unit_id) VALUES ({}); "
                     .format(linguistic_unit_id))
    return mycursor.lastrowid


# group and choose_position are for choose game type
def insert_task_possible_answer(possible_answer_id, task_id, score, group=None, choose_position=None):
    if group is None:
        sql_command = "INSERT INTO task_possible_answer (possible_answer_id, task_id, score) VALUES ({}, {}, {}); "\
            .format(possible_answer_id, task_id, score)
    else:
        if choose_position is None:
            sql_command = "INSERT INTO task_possible_answer (possible_answer_id, task_id, score, group_position) " \
                          "VALUES ({}, {}, {}, {}); " \
                .format(possible_answer_id, task_id, score, group)
        else:
            sql_command = "INSERT INTO task_possible_answer (possible_answer_id, task_id, score, group_position, choose_position) " \
                          "VALUES ({}, {}, {}, {}, {}); "\
                .format(possible_answer_id, task_id, score, group, choose_position)

    mycursor.execute(sql_command)
    return mycursor.lastrowid


class GameType:
    def __init__(self, game_name):
        self.game_id = add_if_not_already(game_name)
        sql_commit()
        self.game_name = game_name
        self.last_call = datetime.datetime.utcnow()

    def __call__(self):
        print(self.game_name, " Time from last call: ", datetime.datetime.utcnow() - self.last_call)
        self.last_call = datetime.datetime.utcnow()


def generate_choose_word(structure_id, headword_pos, task_cycle_id, word_id, word_idx, query_with_word_id=True, word=None):
    collocation_shell = insert_collocation_shell(structure_id)
    word_shell = insert_collocation_shell_word(collocation_shell, word_id, headword_pos)
    task = insert_task(task_cycle_id, collocation_shell, word_idx)

    if query_with_word_id:
        answer_array = get_random_possible_answer_words(word_id, structure_id, game_config["NUMBER_OF_WORDS_IN_ONE_ROUND"])
    else:
        answer_array = get_random_possible_answer_words_not_id(word, structure_id, game_config["NUMBER_OF_WORDS_IN_ONE_ROUND"])
    answer_array.sort(key=lambda tup: tup[3])

    if len(answer_array) < 5:
        print("Choose word too low answer array !!!", len(answer_array))

    # print("word:", word_tuple)
    group_limit = game_config["CHOOSE_NUMBER_OF_WORDS_TO_CHOOSE"]
    sorted_array = [[] for i in range(math.ceil(len(answer_array) / group_limit))]
    shuff_arr = [i for i in range(group_limit)]

    for ans_idx, answer_tuple in enumerate(answer_array):
        answer_id, freq, collocation_id, order_value = answer_tuple
        if ans_idx % group_limit == 0:
            random.shuffle(shuff_arr)

        sorted_array[shuff_arr[ans_idx % group_limit]].append((answer_id, freq, collocation_id, ans_idx))

    group_idx = 0
    for group_array in sorted_array:
        random.shuffle(group_array)
        group_idx += 1
        for answer_tuple in group_array:
            answer_id, freq, collocation_id, ans_idx = answer_tuple

            poss_ans = insert_possible_answer(answer_id)
            # if ans_idx < group_limit:
            insert_task_possible_answer(poss_ans,
                                        task,
                                        CHOOSE_SCORES[math.floor(ans_idx/group_limit)],
                                        group_idx,
                                        ans_idx)
            # else:
            #     task_poss_ans = insert_task_possible_answer(poss_ans,
            #                                             task,
            #                                             CHOOSE_SCORES[math.floor(ans_idx/group_limit)],
            #                                             group_idx)
            # print("answer:", answer_tuple)


class ChooseGameType(GameType):
    def __init__(self):
        super(ChooseGameType, self).__init__("choose")

    def __call__(self, num_of_cycles=1):
        super(ChooseGameType, self).__call__()
        last_cycle = get_proper_cycle_time(self.game_id)
        # print("Choose game type is generating")

        for i in range(num_of_cycles):
            next_cycle = last_cycle + datetime.timedelta(milliseconds=game_config["CHOOSE_DURATION_WHOLE_CYCLE"])
            print("Choose game type is generating", last_cycle)
            task_cycle_id = generate_new_task_cycle(self.game_id, last_cycle, next_cycle)
            last_cycle = next_cycle

            structure_id, headword_pos = get_random_structure()
            word_ids = get_random_headword_from_buff(structure_id, game_config["NUMBER_OF_WORDS_IN_ONE_CYCLE"])

            for idx, word_id in enumerate(word_ids):
                # word_id = word_id[0]
                generate_choose_word(structure_id, headword_pos, task_cycle_id, word_id, idx)

        sql_commit()


def generate_insert_word(structure_id, headword_pos, task_cycle_id, word_id, word_idx):
    collocation_shell = insert_collocation_shell(structure_id)
    word_shell = insert_collocation_shell_word(collocation_shell, word_id, headword_pos)
    task = insert_task(task_cycle_id, collocation_shell, word_idx)

def generate_synonym_word(structure_id, headword_pos, task_cycle_id, word_id, word_idx):
    collocation_shell = insert_collocation_shell(structure_id)
    word_shell = insert_collocation_shell_word(collocation_shell, word_id, headword_pos)
    task = insert_task(task_cycle_id, collocation_shell, word_idx)


class InsertGameType(GameType):
    def __init__(self):
        super(InsertGameType, self).__init__("insert")

    def __call__(self, num_of_cycles=1):
        super(InsertGameType, self).__call__()
        last_cycle = get_proper_cycle_time(self.game_id)
        # print("Insert game type is generating")

        for i in range(num_of_cycles):
            next_cycle = last_cycle + datetime.timedelta(milliseconds=game_config["INSERT_DURATION_WHOLE_CYCLE"])
            print("Insert game type is generating", last_cycle)
            task_cycle_id = generate_new_task_cycle(self.game_id, last_cycle, next_cycle)
            last_cycle = next_cycle

            structure_id, headword_pos = get_random_structure()
            word_ids = get_random_headword_from_buff(structure_id, game_config["INSERT_NUMBER_OF_WORDS_IN_ONE_CYCLE"])

            print(word_ids)

            for idx, word_id in enumerate(word_ids):
                # word_id = word_id[0]
                generate_insert_word(structure_id, headword_pos, task_cycle_id, word_id, idx)

        sql_commit()

class SynonymGameType(GameType):
    def __init__(self):
        super(SynonymGameType, self).__init__("synonym")

    def __call__(self, num_of_cycles=1):
        super(SynonymGameType, self).__call__()
        last_cycle = get_proper_cycle_time(self.game_id)
        # print("Insert game type is generating")

        for i in range(num_of_cycles):
            next_cycle = last_cycle + datetime.timedelta(milliseconds=game_config["SYNONYM_DURATION_WHOLE_CYCLE"])
            print("Synonym game type is generating", last_cycle)
            task_cycle_id = generate_new_task_cycle(self.game_id, last_cycle, next_cycle)
            last_cycle = next_cycle

            #structure_id, headword_pos = get_random_structure()
            structure_id = 10
            headword_pos = 1
            word_ids = get_random_headword_synonym(game_config["SYNONYM_NUMBER_OF_WORDS_RELATED"], game_config["SYNONYM_NUMBER_OF_WORDS_IN_ONE_CYCLE"])

            for idx, word_id in enumerate(word_ids):
                word_id = word_id[0]
                generate_synonym_word(structure_id, headword_pos, task_cycle_id, word_id, idx)

        sql_commit()

class DragGameType(GameType):
    def __init__(self):
        super(DragGameType, self).__init__("drag")
        self.empty_linguistic_id = get_empty_word_id()
        # self.empty_word_id = empty[0]
        # self.empty_structure_id = empty[1]
        sql_commit()
        # One cycle means one drag round so we select only 2 words
        self.num_words = 2

    def __call__(self, num_of_cycles=1):
        super(DragGameType, self).__call__()
        last_cycle = get_proper_cycle_time(self.game_id)
        # print("Drag game type is generating")

        for i in range(num_of_cycles):
            next_cycle = last_cycle + datetime.timedelta(milliseconds=game_config["DRAG_DURATION_WHOLE_CYCLE"])
            print("Drag game type is generating", last_cycle)
            task_cycle_id = generate_new_task_cycle(self.game_id, last_cycle, next_cycle)
            last_cycle = next_cycle

            structure_id, headword_pos = get_random_structure()
            word_ids = get_random_headword_from_buff(structure_id, self.num_words)

            # Append empty word for all wrong adjectives
            # word_ids.append((self.empty_word_id, self.empty_structure_id, word_ids[0][2]))
            num = math.floor(game_config["NUMBER_OF_WORDS_IN_ONE_ROUND"] / (len(word_ids)+1))

            position = [i for i in range(game_config["DRAG_NUMBER_OF_WORDS_IN_ONE_ROUND"])]
            random.shuffle(position)

            word_ids_to_ling = {self.empty_linguistic_id:self.empty_linguistic_id}
            for word_id in word_ids:
                # word_id = word_id[0]
                word_ids_to_ling[word_id] = get_linguistic_id(word_id)

            word_ids.append(self.empty_linguistic_id)
            # print(word_ids, "linguistic:", word_ids_to_ling)

            idx = 0
            for word_id in word_ids:
                # word_id = word_id[0]
                if word_id == self.empty_linguistic_id:
                    answer_array = get_random_words(num)
                else:
                    answer_array = get_random_possible_answer_words(word_id, structure_id, num, True)
                    answer_array.sort(key=lambda tup: tup[1], reverse=True)
                # print(answer_array)

                # print("word:", word_tuple)
                for ans_idx, answer_tuple in enumerate(answer_array):
                    if word_id == self.empty_linguistic_id:
                        answer_id = answer_tuple[0]
                    else:
                        answer_id, freq, collocation_id, order_value = answer_tuple
                    # print("answer_id:", answer_id)

                    collocation_shell = insert_collocation_shell(structure_id)
                    word_shell = insert_collocation_shell_word(collocation_shell, answer_id, (headword_pos-1)*(-1)+2)
                    task = insert_task(task_cycle_id, collocation_shell, position[idx])
                    idx += 1

                    for w_id in word_ids:
                        poss_ans = insert_possible_answer(word_ids_to_ling[w_id])
                        score = 0
                        if w_id == self.empty_linguistic_id:
                            score = DRAG_SCORES[1]
                        if w_id == word_id:
                            score = DRAG_SCORES[0]
                        task_poss_ans = insert_task_possible_answer(poss_ans, task, score)
                        # print("answer:", answer_tuple)

        sql_commit()


if __name__ == "__main__":
    init_start = datetime.datetime.utcnow()
    init_random_headword_buffer()
    init_diff = datetime.datetime.utcnow() - init_start
    print("Random hedword buffer initialized", init_diff)
    # ADD ONE NEW CYCLE WITH CHOOSE GAME TYPE
    ChooseGame = ChooseGameType()
    # ADD ONE NEW CYCLE WITH CHOOSE GAME TYPE
    InsertGame = InsertGameType()
    # ADD ONE NEW CYCLE WITH CHOOSE GAME TYPE
    DragGame = DragGameType()
    # ADD ONE NEW CYCLE WITH CHOOSE GAME TYPE
    SynonymGame = SynonymGameType()

    generate_cycles_ahead = 10
    ChooseGame(generate_cycles_ahead)
    InsertGame(generate_cycles_ahead)
    DragGame(generate_cycles_ahead)
    SynonymGame(generate_cycles_ahead)

    schedule.every((game_config["CHOOSE_DURATION_WHOLE_CYCLE"]/1000) - 1).seconds.do(ChooseGame)
    schedule.every((game_config["INSERT_DURATION_WHOLE_CYCLE"]/1000) - 1).seconds.do(InsertGame)
    schedule.every((game_config["DRAG_DURATION_WHOLE_CYCLE"]/1000) - 1).seconds.do(DragGame)
    schedule.every((game_config["SYNONYM_DURATION_WHOLE_CYCLE"]/1000) - 1).seconds.do(SynonymGame)

    while True:
        schedule.run_pending()
        time.sleep(1)
