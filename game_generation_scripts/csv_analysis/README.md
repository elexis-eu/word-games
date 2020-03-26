This directory contains two Python 3 scripts (make_lists.py,
analyse_csv.py) for basic analysis of a collocations CSV file,
which could be helpful during data preparation.

## make_lists.py

make_lists.py takes a CSV collocations file (-infile) and creates a
directory (-outdir) with headword lists for each structure. The input
file should list one line per collocation and have the following columns:

```
id|structure|position|word1|word2|frequency|logDice|weight|variants
```

The 2nd column gives the structure name, and the collocation headword
is in either the 4th or 5th column, depending on whether the value of
the 3rd column is 1 or 2, respectively.

The script creates the -outdir directory (first *deleting* it, if it
already exists) and fills it with one file per structure. Each
structure file simply lists all the unique headwords from the input
file for that structure. The structure file names have the format
{structure_name}__{position}.txt.

The script is run like this:

```shell
$ python3 make_lists.py -infile collocations_dutch.csv -outdir lists_dutch
```

## analyse_csv.py

analyse_csv.py conducts a basic analysis of the overlap in the number
of collocations for each pair of headwords of each structure. As
input, it takes a CSV collocations file (-csvfile) and a directory
with headword lists by structure (-listdir), and creates a directory
with the results (-outdir).

Normally, -csvfile and -listsdir have the
same values as -infile and -outdir from make_lists.py, unless you want
to run a further custom script of your own in between (e.g., dividing
the structure lists by gender or some other grammatical feature).

For each structure, the script takes each pair of headwords and counts
the number of collocations for the first headword, the number for the
second, and the number in common. The structure's output file 
lists one line per headword pair with these columns:

```
structure|position|headword1|headword2|collocations1|collocations2|collocationsBoth
```

The script is run like this:

```shell
$ python3 analyse_csv.py -csvfile collocations_dutch.csv -listsdir lists_dutch -outdir analyses_dutch
```
