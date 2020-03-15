#!/usr/bin/env python3

import sys
import argparse
import random


class Game:
    def __init__(self, data_file, min_collocates, min_joint_collocates):
        self.pairs = []
        with open(data_file) as f:
            for i, line in enumerate(f):
                if i == 0:
                    continue
                tokens = line[:-1].split('|')
                tokens[1] = int(tokens[1])
                tokens[4] = int(tokens[4])
                tokens[5] = int(tokens[5])
                tokens[6] = int(tokens[6])

                if tokens[4] < min_collocates or tokens[5] < min_collocates:
                    continue

                if tokens[4] + tokens[5] < min_joint_collocates:
                    continue

                # Overlay ratio
                overlay_ratio1 = tokens[6] / tokens[4]
                overlay_ratio2 = tokens[6] / tokens[5]

                if overlay_ratio1 > 0.5 or overlay_ratio2 > 0.5:
                    continue
                
                if overlay_ratio1 < 0.15 and overlay_ratio2 < 0.15:
                    overlay_ratio = 'l'
                else:
                    overlay_ratio = 'h'

                tokens.append(overlay_ratio)
                tokens.append(overlay_ratio1)
                tokens.append(overlay_ratio2)
                self.pairs.append(tuple(tokens))

    def generate_levels(self, n, lvl_size, overlay_level):
        random.shuffle(self.pairs)
        pairs = self.__arrange_pairs(self.pairs, overlay_level)
        self.__generate_levels(pairs[:n*lvl_size], lvl_size)

    def __generate_levels(self, pairs, lvl_size, prev_game_left=0, level_num=0):
        duplicate_pairs = []
        unique_headwords = set()

        done_cntr = lvl_size - prev_game_left
        for i in range(len(pairs)):
            start = False
            end = False

            if done_cntr % lvl_size == 0:
                start = True
                level_num += 1
                unique_headwords = set()
                done_cntr = 0
                # print('')
            elif done_cntr % lvl_size == lvl_size - 1:
                end = True

            pair = pairs[i]
            headword1 = pair[2]
            headword2 = pair[3]

            if headword1 in unique_headwords or \
                (not start and not end and headword2 in unique_headwords):
                duplicate_pairs.append(pair)
                continue

            done_cntr += 1
            self.__print_pair(pair, level_num, done_cntr, start=start, end=end)

            unique_headwords.add(headword1)
            if not start and not end:
                unique_headwords.add(headword2)


        if len(duplicate_pairs) > 0:
            if not end:
                prev_game_left = lvl_size - (done_cntr % lvl_size)
                if prev_game_left == lvl_size:
                    prev_game_left = 0
            self.__generate_levels(duplicate_pairs, lvl_size, prev_game_left, level_num)

    def __print_pair(self, pair, level_num, pair_num, start=False, end=False):
        res = []
        res.append(str(level_num))
        
        if start:
            res.append(';choose;')
        elif end:
            res.append(';insert;')
        else:
            res.append(';drag;')

        res.append(pair[0])
        res.append(';')
        res.append(pair[2])
        res.append(';')
        if not start and not end:
            res.append(pair[3])
        res.append(';')
        res.append(str(pair_num))

        # Debug
        res.append(';')
        res.append(str(pair[-2]))
        res.append(';')
        res.append(str(pair[-1]))
        res.append(';')
        res.append(str(pair[-3]))

        print(''.join(res))

    def __arrange_pairs(self, pairs, overlay_level):
        if overlay_level == 'high':
            pairs = sorted(pairs, key=lambda tup: tup[7])
        elif overlay_level == 'low':
            pairs = sorted(pairs, key=lambda tup: tup[7], reverse=True)
        elif overlay_level == 'mixed':
            pairs = sorted(pairs, key=lambda tup: tup[7], reverse=True)
            half = len(pairs) // 2
            tmp = []
            for i in range(len(pairs)):
                if i == half:
                    break
                tmp.append(pairs[i])
                tmp.append(pairs[-i])
            pairs = tmp
        elif overlay_level == 'random':
            random.shuffle(pairs)
        else:
            print("Wrong argument for overlay_level")
            sys.exit(1)
        return pairs



if __name__ == '__main__':
    arg_parser = argparse.ArgumentParser()
    arg_parser.add_argument("--num-pairs", type=int, default=400,
                            help="Number of pairs to generate.")
    arg_parser.add_argument("--lvl-len", type=int, default=4,
                            help="Number of pairs per level.")
    arg_parser.add_argument("--min-collocates", type=int, default=0,
                            help="Minimum number of collocates per headword.")
    arg_parser.add_argument("--min-joint-collocates", type=int, default=8, 
                            help="Minimum number of joint collocates.")
    arg_parser.add_argument("--collocator-overlay-level", type=str, default='random',
                            help="Prefered collocator overlay level. Options are: low, mixed, high, random")
    arg_parser.add_argument("input_file")
    args = arg_parser.parse_args()

    n = arg.num_pairs
    lvl_size = arg.lvl_len

    if n <= 0 or lvl_size <= 0 or lvl_size > n or n % lvl_size != 0:
        print("Faulty parameters.")
        sys.exit(1)

    game = Game(args.input_file, args.min_collocates, args.min_joint_collocates)
    game.generate_levels(n // lvl_size, lvl_size, args.collocator_overlay_level)
