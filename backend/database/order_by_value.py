import mysql.connector
import collections
import datetime

mydb = mysql.connector.connect(
    host="localhost",
    port="3306",
    user="igrabesed",
    passwd="Igrapass!",
    database="igra_besed"
)

mycursor = mydb.cursor()

def sql_commit():
    mydb.commit()



structures = {}
random_headword_buffer = {}
MIN_ARRAY_LENGTH = 30
isAll = {}


def init_random_headword_buffer():
    all_structures = get_all_structures()
    for structure in all_structures:
        s_id, headword_pos = structure
        structures[s_id] = headword_pos
        isAll[s_id] = False
        fill_random_headword_buffer(s_id, headword_pos)


def get_random_headword_from_buff(structure_id):
    if len(random_headword_buffer[structure_id]) > 0:
        result = random_headword_buffer[structure_id].popleft()
    else:
        result = None

    if not isAll[structure_id] and len(random_headword_buffer[structure_id]) < MIN_ARRAY_LENGTH:
        fill_random_headword_buffer(structure_id, structures[structure_id])
    return result


def fill_random_headword_buffer(s_id, headword_pos):
    word_ids = get_random_headword(headword_pos, s_id, 1000)
    # print(word_ids)
    if word_ids is None or len(word_ids) == 0:
        print("all headword words filled, structure: ", s_id)
        isAll[s_id] = True
    random_headword_buffer[s_id] = collections.deque()
    for w_id in word_ids:
        w_id = w_id[0]
        random_headword_buffer[s_id].append(w_id)


def get_all_structures():
    mycursor.execute("SELECT s.id, s.headword_position  \
                        FROM structure as s;")
    r = mycursor.fetchall()
    return r


def get_random_headword(pos, structure_id, num):
    mycursor.execute("SELECT cw.word_id     \
                        FROM collocation_word as cw                         \
                        JOIN collocation as c                               \
                        ON c.id = cw.collocation_id                         \
                        WHERE cw.position = {} AND c.structure_id = {} AND c.order_value IS NULL       \
                        GROUP BY cw.word_id                                 \
                        HAVING COUNT(cw.word_id) > 8                        \
                        LIMIT {};".format(pos, structure_id, num))
    r = mycursor.fetchall()
    # print("grh: ", r)
    return r


def get_collocation_words(word_id, structure_id):
    mycursor.execute("select c.id   \
                        from word as w      \
                        join word as w2     \
                        join collocation_word as cw     \
                        join collocation_word as cw2        \
                        join collocation as c   \
                        on cw.collocation_id=cw2.collocation_id and c.id=cw.collocation_id and w.id = cw.word_id  \
                                and cw2.word_id=w2.id and w.id!=w2.id \
                        where w.id={} and c.structure_id={}      \
                        order by c.sailence desc;".format(word_id, structure_id))
    r = mycursor.fetchall()
    return r


def insert_order_value(collocation_id, value):
    mycursor.execute("UPDATE collocation as c SET c.order_value = {} WHERE c.id = {} ".format(value, collocation_id))


if __name__ == "__main__":
    init_start = datetime.datetime.now()
    init_random_headword_buffer()
    init_diff = datetime.datetime.now() - init_start
    print("Headword buffer initialized", init_diff)
    #global isAll
    all_structures = get_all_structures()
    number = 1
    for structure in all_structures:
        s_id, headword_pos = structure
        headword = get_random_headword_from_buff(s_id)
        while headword is not None:
            print(headword)
            words = get_collocation_words(headword, s_id)
            i = 1.0
            length = len(words)
            for word in words:
                insert_order_value(word[0], i/length)
                i = i + 1.0
            number += 1
            if number > 1000:
                print("sql commit")
                sql_commit()
                number = 0
            headword = get_random_headword_from_buff(s_id)

print("sql commit")
sql_commit()
