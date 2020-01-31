use igra_besed;

SELECT COUNT(w.id) 
	from collocation_word as w;

select * from lemma as l 
		join word as w
        on w.lemma_id = l.id
		where l.id = 22460;

select * from word as w
		join collocation_word as cw
        join collocation as c
        on cw.word_id = w.id and c.id = cw.collocation_id
        where cw.collocation_id = 1473663;
        
select w.linguistic_unit_id from word as w
	where w.id = 2134;
    
    
SELECT DISTINCT *
		FROM word as w                                             
		ORDER BY RAND() LIMIT 5;
        
    
SELECT *
		FROM word as w   
        join collocation_word as cw
        join collocation as c
        join structure as s
		on w.id = cw.word_id and cw.collocation_id = c.id and c.structure_id = s.id
        where w.id=160447;
        
select * from task_cycle;

select * from task_type;

select * from word as w
		join collocation_word as cw
        on w.id=cw.word_id
        where cw.collocation_id=6903352;
    
select *
	from word as w
    join word as w2
	join collocation_word as cw
	join collocation_word as cw2
    on w.id=cw.word_id and cw2.word_id=w2.id and w.id!=w2.id and cw.position=1 and cw2.position=2
    where cw.collocation_id=3420818 and cw2.collocation_id=3420818;
    
select *  
		from word as w
		join word as w2
		join collocation_word as cw
		join collocation_word as cw2
        join collocation as c
		on cw.collocation_id=cw2.collocation_id and c.id=cw.collocation_id and w.id = cw.word_id  and cw2.word_id=w2.id and w.id!=w2.id
        where w.text="izobražen" and w2.text="fakultetno" ; # and c.structure_id=9;
        
        
select w2.text, c.frequency, c.sailence, c.order_value, c.id
		from word as w
		join word as w2
		join collocation_word as cw
		join collocation_word as cw2
        join collocation as c
		on cw.collocation_id=cw2.collocation_id and c.id=cw.collocation_id and w.id = cw.word_id  and cw2.word_id=w2.id and w.id!=w2.id
        where w.text="miza" and c.structure_id=9 # w.id=77647 and c.structure_id=1  #	w.text = "poslej"
        order by c.sailence desc;
        
select *
    from collocation as c
    where c.order_value = 0 limit 5;
    
SELECT cw.word_id, count(cw.word_id)
	FROM collocation_word as cw
    JOIN collocation as c
    ON c.id = cw.collocation_id
    WHERE cw.position = 2 AND c.structure_id = 1 
    GROUP BY cw.word_id
    HAVING COUNT(cw.word_id) > 8
    ORDER BY RAND() LIMIT 5;
    
        
select *
	from task_possible_answer as tpa
    where tpa.id = 55607;
    
SELECT count(w.uid) as curr_max from user as w;
    
select u.display_name, tu.thematic_score, tu.thematic_position, u.uid, tu.thematic_id
	from thematic_user as tu
    join user as u
    where u.uid = tu.user_id
    order by tu.thematic_score desc limit 100;
    
select u.display_name, u.choose_score, (SELECT COUNT(*) + 1 FROM user x WHERE x.choose_score > u.choose_score) AS position
	from user as u
    where u.uid = "lolek23";

UPDATE user as u SET u.thematic_position = 0 WHERE u.uid = "neki";
select * from user as u;
    
select * from task_type;
select * from word as w where w.text = 'potovanje';

    
# ALTER TABLE collocation ADD COLUMN order_value FLOAT(8,5) NOT NULL;
# ALTER TABLE user ADD COLUMN choose_score INT(11) NOT NULL DEFAULT  0;
# ALTER TABLE user ADD COLUMN insert_score INT(11) NOT NULL DEFAULT  0;
# ALTER TABLE user ADD COLUMN drag_score INT(11) NOT NULL DEFAULT  0;
# ALTER TABLE structure ADD COLUMN text VARCHAR(80) NOT NULL ;
ALTER TABLE task_cycle ADD COLUMN thematic_id INT(11) NULL,
	ADD FOREIGN KEY (thematic_id) REFERENCES thematic(id);
ALTER TABLE task_cycle ADD COLUMN index_order INT(11) NULL;
ALTER TABLE task_cycle DROP COLUMN index_order;

ALTER TABLE user ADD COLUMN age VARCHAR(45) NULL;
ALTER TABLE user ADD COLUMN native_language VARCHAR(45) NULL;
ALTER TABLE user ADD COLUMN thematic_score	INT(11) NOT NULL DEFAULT  0;
ALTER TABLE user ADD COLUMN thematic_position INT(11) NOT NULL DEFAULT  0;
ALTER TABLE user DROP COLUMN thematic_score;
ALTER TABLE user DROP COLUMN thematic_position;

ALTER TABLE user ADD COLUMN sum_score	INT(11) NOT NULL DEFAULT  0;
update user as u SET u.sum_score = u.choose_score + u.insert_score + u.drag_score;


UPDATE structure as s SET s.text = "prislov + glagol" WHERE s.id = 3;
UPDATE structure as s SET s.text = "prislov + glagol" WHERE s.id = 4;

UPDATE structure as s SET s.text = "prislov + pridevnik" WHERE s.id = 2;
UPDATE structure as s SET s.text = "prislov + pridevnik" WHERE s.id = 8;

UPDATE structure as s SET s.text = "glagol + samostalnik v tožilniku" WHERE s.id = 1;
UPDATE structure as s SET s.text = "glagol + samostalnik v tožilniku" WHERE s.id = 6;

UPDATE structure as s SET s.text = "pridevnik + samostalnik" WHERE s.id = 5;
UPDATE structure as s SET s.text = "pridevnik + samostalnik" WHERE s.id = 7;

UPDATE structure as s SET s.text = "samostalnik + samostalnik v rodilniku" WHERE s.id = 9;


select * from task_type;
select * from thematic;
select * from structure;
select * from user as u where u.display_name='Stroj14';
select *, UNIX_TIMESTAMP(tc.from_timestamp)  from task_cycle as tc where tc.task_type_id = 4 ORDER BY tc.from_timestamp;
select * from user limit 10;
# insert into task_type (name) values ("thematic");

# change password:
# SET PASSWORD FOR 'root'@'localhost' = PASSWORD('root1');

