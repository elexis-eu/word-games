SET @current_time = NOW();
SET @game_type=1;
SELECT @@global.time_zone;
SET @@global.time_zone = '+01:00';

SELECT UNIX_TIMESTAMP(), NOW();

# Pridobimo naslednji cikel za določen tip igre
SELECT tc.id, tc.from_timestamp, tc.to_timestamp FROM task_cycle AS tc
  WHERE tc.from_timestamp > @current_time
    AND tc.task_type_id=@game_type
  ORDER BY tc.from_timestamp limit 10;


SET @task_cyc_id = 12002;
# Najdemo vse taske za ta cikel
# Najdemo besede, ki se prikažejo
SELECT t.id, t.position as "t_position", w.text, csw.position as "csw_position", cs.structure_id as "structure_id", s.text as "structure_text" FROM task AS t
	JOIN collocation_shell AS cs
  JOIN collocation_shell_word AS csw
  JOIN word AS w
  JOIN structure as s
  ON cs.id = t.collocation_shell_id
    AND csw.collocation_shell_id = t.collocation_shell_id
    AND w.id = csw.word_id
    AND s.id = cs.structure_id
	WHERE t.task_cycle_id=@task_cyc_id
  ORDER BY t.position;



# Dobili smo vse task_id => primer: "delo"
SET @task_id=65148;

# poiščemo še vse možne odgovore:
SELECT tpa.id, w.text, tpa.score, tpa.group_position FROM task_possible_answer AS tpa
	JOIN possible_answer AS pa
  JOIN linguistic_unit AS lu
  JOIN word AS w
  ON tpa.possible_answer_id=pa.id
    AND pa.linguistic_unit_id=lu.id
    AND w.linguistic_unit_id=lu.id
	WHERE tpa.task_id=@task_id
  ORDER BY tpa.score DESC;