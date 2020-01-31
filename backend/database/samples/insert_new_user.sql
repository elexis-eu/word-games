
# Nov uporabnik
INSERT INTO user (username, password, display_name, created, modified)
	VALUES ("Stroj14", "koda123", "Stroj14", NOW(), NOW());
SET @new_user = LAST_INSERT_ID();  

# Igral je task 5 
INSERT INTO task_user (task_id, user_id)
	VALUES (5, @new_user);


# UPDATE: task_answer bi se lahko direktno vezal na possible_answer, ker imamo že podatek o task_id


UPDATE user as u SET choose_score = 30 WHERE u.uid = "lolek23";