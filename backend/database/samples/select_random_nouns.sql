SELECT s.id, s.headword_position
	FROM structure as s
    ORDER BY RAND() LIMIT 1;

SELECT s.id, s.headword_position
	FROM structure as s;

SELECT cw.word_id, count(cw.word_id)
	FROM collocation_word as cw
    JOIN collocation as c
    ON c.id = cw.collocation_id
    WHERE cw.position = 2 AND c.structure_id = 1 
    GROUP BY cw.word_id
    HAVING COUNT(cw.word_id) > 8
    ORDER BY RAND() LIMIT 5;
    
    
SELECT DISTINCT cw.word_id
	FROM collocation_word AS cw 
	JOIN collocation as c
	JOIN
       (SELECT CEIL(RAND() *
                     (SELECT MAX(id)
                        FROM collocation_word)) AS id)
        AS r2
	ON c.id = cw.collocation_id
	WHERE cw.id >= r2.id AND cw.position = 2 AND c.structure_id = 5
	ORDER BY cw.word_id ASC
	LIMIT 5;
