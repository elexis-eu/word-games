A Python3 script used to generate verb game levels.

The script reads verb pairs from a file (in the format output by analyse_csv.py), passed to the script as an argument.

It filters out pairs with verbs, where both verbs have overlay ratios of over 0.5.
An overlay ratio of a verb is calculated by dividing the number of combined collocates of the pair by the number of collocates of the first verb.

From the pairs left over, the ones where both verbs have overlay ratios of less than 0.15, get labeled as low ("l"), and the others as high ("h").

The resulting set of pairs will be sorted, according to the "--collocator-overlay-level" (see below).

Note that while the script generates the game, pairs which contain a verb, which already appeared before, they get pushed to the end part of the game.

## Usage
To see all the possible options, run:

```shell
./create_verb_game.py --help
```

### Perfered overlay level
With the --collocator-overlay-level you can choose how the valid set of pairs gets sorted, from which the first n pairs will be used to create levels.
Possible values include: low, high, mixed, random

"low" sorts the pairs, so that pais with the "l" label appear first and "h" appear after them, whereas the "high" value works the opposite way.
"random" just shuffles the set of pairs.
With the "mixed" value, the pairs are arranged, so that their overlay levels are alternating. For example one "l", one "h" and so on, until only one type is left.


A usage example, that creates levels from 1200 pairs, where each level has 6.
The script filters out pairs, that have less than 10 collocates and less than 30 joint collocates.
The collocator overlay level is set to "low", so the pairs labeled as "l" get used first.

```shell
./create_verb_game.py --num-pairs 1200 --lvl-len 6 --min-collocates 10 --min-joint-collocates 30 --collocator-overlay-level low gbz_SBZ4__2.csv
```

## Output
The output of the script will be formated the following way:
```
level number;move type;pair structure;verb1;verb2;move number;overlay ratio of verb1;overlay ratio of verb2;overlay level
```
