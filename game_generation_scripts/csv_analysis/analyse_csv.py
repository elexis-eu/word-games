#!/usr/bin/python3

import argparse
import codecs
import itertools
import os
import shutil
import glob
import re

arg_parser = argparse.ArgumentParser(description='Analyse number of collocates and overlaps (#1112).')
arg_parser.add_argument('-csvfile', type=str, help='Input file')
arg_parser.add_argument('-listsdir', type=str, help='Directory with lists of headwords')
arg_parser.add_argument('-outdir', type=str, help='Output file')
arguments = arg_parser.parse_args()
csv_file_name = arguments.csvfile
list_directory = arguments.listsdir
output_directory = arguments.outdir

shutil.rmtree(output_directory,True)
os.makedirs(output_directory)

data_map = {}

def get_key(structure, position):
    return structure + '__' + str(position)

csv_file = codecs.open(csv_file_name, 'r')
next(csv_file)
for line in csv_file:
    if ('|' in line):
        columns = line.strip().split('|')
        structure = columns[1]
        position = int(columns[2])
        headword = columns[3] if position == 1 else columns[4]
        collocate = columns[3] if position == 2 else columns[4]
        key = get_key(structure, position)
        if (key not in data_map.keys()):
            data_map[key] = {}
        if (headword not in data_map[key].keys()):
            data_map[key][headword] = set()
        data_map[key][headword].add(collocate)
csv_file.close()

list_file_pattern = re.compile('^(.*?)__(.*?)(?:__.*)?\.txt$')

list_file_names = glob.glob(list_directory + '/*__*.txt')
for list_file_name in list_file_names:

    base_file_name = os.path.basename(list_file_name)

    match = list_file_pattern.match(base_file_name)
    [structure, position] = match.groups()

    headwords = set()
    list_file = codecs.open(list_file_name, 'r')
    for line in list_file:
        headwords.add(line.strip())
    list_file.close()

    key = get_key(structure, position)
    structure_map = data_map[key]

    output_file_name = output_directory + '/' + base_file_name[:-4] + '.csv'
    output_file = codecs.open(output_file_name, 'w')
    header_columns = ['Structure', 'Position', 'Headword1', 'Headword2', 'Collocates1', 'Collocates2', 'CollocatesBoth']
    output_file.write('|'.join(header_columns) + '\n')
    for (headword1, headword2) in itertools.combinations(sorted(headwords), 2):
        headword1_count =  len(structure_map[headword1])
        headword2_count =  len(structure_map[headword2])
        joint_count =  len(structure_map[headword1] & structure_map[headword2])
        columns = [structure, str(position), headword1, headword2, str(headword1_count), str(headword2_count), str(joint_count)]
        output_file.write('|'.join(columns) + '\n')
    output_file.close()
