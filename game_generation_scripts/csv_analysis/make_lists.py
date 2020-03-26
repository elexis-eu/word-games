#!/usr/bin/python3

import argparse
import codecs
import shutil
import os

arg_parser = argparse.ArgumentParser(description='Extract headword lists by structure (#1112).')
arg_parser.add_argument('-infile', type=str, help='Input file')
arg_parser.add_argument('-outdir', type=str, help='Output directory ')
arguments = arg_parser.parse_args()
input_file_name = arguments.infile
output_directory = arguments.outdir

shutil.rmtree(output_directory,True)
os.makedirs(output_directory)

data_map = {}

input_file = codecs.open(input_file_name, 'r')
next(input_file)
for line in input_file:
    if ('|' in line):
        columns = line.strip().split('|')
        structure = columns[1]
        position = int(columns[2])
        headword = columns[3] if position == 1 else columns[4]
        key = (structure, position)
        if (key not in data_map.keys()):
            data_map[key] = set()
        data_map[key].add(headword)
input_file.close()

for key in data_map.keys():
    (structure, position) = key
    structure_file_name = output_directory + '/' + structure + '__' + str(position) + '.txt'
    structure_file = codecs.open(structure_file_name, 'w')
    for headword in sorted(data_map[key]):
        structure_file.write(headword + '\n')
    structure_file.close()
