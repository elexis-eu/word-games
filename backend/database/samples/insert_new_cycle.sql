
# TASK tip.
INSERT INTO task_type (name) VALUES ("choose");
SET @task_type_id = LAST_INSERT_ID();

# TASK CIKEL, ki ima n taskov.
INSERT INTO task_cycle (task_type_id) VALUES (@task_type_id);
SET @task_cycle_last_id = LAST_INSERT_ID();    

# Besede, ki se prikažejo na zaslon (v tem primeru je to krog)
SET @structure_id = 1;
INSERT INTO collocation_shell (structure_id) VALUES (@structure_id);
SET @collocation_shell_last_id = LAST_INSERT_ID();    

SET @word_krog_id = 1312;
SET @position = 2;
INSERT INTO collocation_shell_word (collocation_shell_id, word_id, position)
		VALUES (@collocation_shell_last_id, @word_krog_id, @position);


# Task je narjen. Ima collocation_shell_id, ki pove katere besede se prikažejo ter task_cycle_id
INSERT INTO task (task_cycle_id, collocation_shell_id, position)
		VALUES (@task_cycle_last_id, @collocation_shell_last_id, @position);
SET @task_last_id = LAST_INSERT_ID();    


# Dodamo 2 možna odgovora (voden in filmski)
SET @linguistic_unit_voden_id = 1400;
INSERT INTO possible_answer (linguistic_unit_id) VALUES (@linguistic_unit_voden_id);
SET @possible_answer_voden = LAST_INSERT_ID(); 
   
SET @linguistic_unit_filmski_id = 1206;
INSERT INTO possible_answer (linguistic_unit_id) VALUES (@linguistic_unit_filmski_id);
SET @possible_answer_filmski = LAST_INSERT_ID();    

# JE RES TAKO TREBA??? za vsak possible answer nova tabela task_possible_answer
INSERT INTO task_possible_answer (possible_answer_id, task_id, score)
		VALUES (@possible_answer_voden, @task_last_id, 100);

INSERT INTO task_possible_answer (possible_answer_id, task_id, score)
		VALUES (@possible_answer_filmski, @task_last_id, 100);
        



