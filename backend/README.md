# Word games - NodeJS/Python3 backend

## Tested platform
- Windows 10
- NodeJS v12.4.1
- MySQL 8.0.17
- Python 3.7.6

# Preparing 

## MySQL database
- create new schema and user for specific database that will be used only for one language [expert way](https://dev.mysql.com/doc/refman/8.0/en/creating-accounts.html) / [GUI way](https://www.mysql.com/products/workbench/)
- pick charset and collation best suited for planned language
- import tables and basic task/game types using file **database/setup.sql**

## Python import scripts
- use command **pip install mysql-connector-python** to install MySQL connector for Python
- **every script** has credentials for MySQL which **must be set** before use `mydb = mysql.connector.connect(
host="localhost",
port="3305",
user="user",
passwd="password",
database="new_schema"
)`
 

## Importing collocations vocabulary
### Importing general vocabulary with Lexeme IDs

Python script **load_data.py** in *database* folder will import main vocabulary for collocations.
Filename can be set in *csv_filename* variable, must be in UTF-8 encoding and delimited with character | .

Before importing define *structures* tuples:
Four fields  represent:

    Unique ID, structure code, headword position (1 or 2), text that will be shown in app 

CSV file should have following fields:

 - Collocation ID (integer)
 - Structure code ( look structure tuple )
 - Headword position (1 or 2) 
 - Lexeme ID 1 (integer)
 - Word 1 (text max 255 characters)
 - Lexeme ID 2 (integer) 
 - Word 2 (text max 255 characters)
 - Frequency (integer)
 - Sailence (decimal value)
 - Headword variants (text max 255 characters, separated by /)

 Reimporting whole file is not viable, in case of error database will have to be cleared  or restarted from last successful line imported.
 
### Importing general vocabulary without Lexeme IDs

Python script **load_data_no_lexeme.py** in *database* folder will import main vocabulary for collocations.
Filename can be set in *csv_filename* variable, must be in UTF-8 encoding and delimited with character | .

Before importing define *structures* tuples:
Four fields  represent:

    Unique ID, structure code, headword position (1 or 2), text that will be shown in app 

CSV file should have following fields:

 - Collocation ID (integer)
 - Structure code ( look structure tuple )
 - Headword position (1 or 2) 
 - Word 1 (text max 255 characters)
 - Word 2 (text max 255 characters)
 - Frequency (integer)
 - LogDice (decimal value)
 - Specific weight (integer, adding this number will lower collocation priority after reaching weight limit - 20 by default)
 - Headword variants (text max 255 characters, separated by /)

 Reimporting whole file is not viable, in case of error database will have to be cleared or restarted from last successful line imported.

### Calculating order values
After succesful import of vocabulary run Python script **order_by_value.py** in *database* folder to calculate order values that are used as base for scoring.

### Importing collocations levels for SOLO mode
Run Python script **set_collocations_solo_level_headwords.py** in *database* folder.
CSV should be named **collocations_level_headwords.csv**, delimited with comma (,) and in UTF-8 encoding.

The mandatory fields are:

 - Level (integer)
 - Game type ('choose', 'insert' or 'drag')
 - Structure code
 - Headword1 (used in choose and insert)
 - Headword2 (used in drag)
 - Position (position in level)

 Reimporting file will deactivate previous levels based on **level+position**.

### Importing collocations level title for SOLO mode
Run Python script **set_collocations_solo_level_title.py** in *database* folder.
CSV should be named **collocations_level_title.csv**, delimited with comma (,) and in UTF-8 encoding.

The mandatory fields are:

- Level (integer)
- Title (text max 255 characters)
- Next round game limit (from 1 to 10)

 Reimporting file will overwrite previous fields based on **level**.

### Importing thematic levels
Run Python script **thematic_generator.py** in *game_generator* folder.

Themes are imported from **thematic.txt** file.



## Importing synonyms vocabulary

### Importing main vocabulary
Run Python script **load_synonym.py** in *database* folder.
XML file should be named **CJVT_Thesaurus-v1.0.xml** (can be changed in the script) and in UTF-8 encoding.

XML example file is named **CJVT_Thesaurus-v0.1.xml**. 

Can be used for reimport, it is checking previous headwords.

### Importing levels for SOLO mode

Run Python script **set_synonym_level.py** in *database* folder.
CSV should be named **synonym_headwords_levels.csv**, delimited with comma (,) and in UTF-8 encoding.

Mandatory fields are:

- Headword (must exists in vocabulary or wont be imported)
- Level (integer)

Can be used for reimport if all headwords are the same, just changing levels.

# Running server

## Enviroment variables

Set this enviroment variables if you will run multiple language endpoints on the same server instance.

- IGRA_BESED_DATABASE_HOST - MySQL database hostname
- IGRA_BESED_DATABASE_PORT - MySQL database port
- IGRA_BESED_DATABASE_USER - MySQL database user
- IGRA_BESED_DATABASE_PASSWD - MySQL database password
- IGRA_BESED_DATABASE_SCHEMA - MySQL database schema
- IGRA_BESED_SERVER_KEY - HTTPS private key location
- IGRA_BESED_SERVER_CERT - HTTPS  certificate location
- IGRA_BESED_SERVER_ADDRESS - endpoint server hostname
- IGRA_BESED_SERVER_PORT - endpoint server port


## Game generator

Games for competition mode in collocations are based on time and need to have game generator running to create new instance of games.

Run Python script **game_generator.py** in *game_generator* folder. Recommended to make a daemon for it.

## NodeJS web server

Install necessary modules with **npm install**.
Default MySQL credentials can be set in file "game/Query,js" or use enviroment variables.
Default hostname is localhost(127.0.0.1) and port 3000, which can be changed in **server.js** file.
Start server with **npm start** or **node start.js**.

For development reasons HTTPS part is commented out.
