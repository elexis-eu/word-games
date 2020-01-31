SELECT DISTINCT w.linguistic_unit_id, c.frequency, cw.collocation_id
	FROM collocation_word as cw
	JOIN word as w
    JOIN collocation as c
    JOIN structure as s
    ON cw.collocation_id=c.id AND w.id=cw.word_id AND s.id = c.structure_id
    WHERE cw.position!=s.headword_position  AND c.id IN (SELECT cw.collocation_id
											FROM collocation_word as cw
	                                        JOIN word as w
                                            ON w.id=cw.word_id)
    ORDER BY RAND() LIMIT 5;