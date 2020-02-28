-- MySQL dump 10.13  Distrib 8.0.17, for Win64 (x86_64)
--
-- Host: localhost    Database: igra_besed
-- ------------------------------------------------------
-- Server version	8.0.17

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `collocation`
--

DROP TABLE IF EXISTS `collocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `form` varchar(64) NOT NULL,
  `frequency` int(11) NOT NULL,
  `sailence` float(8,5) DEFAULT NULL,
  `order_value` float(8,5) DEFAULT NULL,
  `status` int(11) DEFAULT NULL,
  `structure_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `structure_id` (`structure_id`),
  CONSTRAINT `collocation_ibfk_1` FOREIGN KEY (`structure_id`) REFERENCES `structure` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_level`
--

DROP TABLE IF EXISTS `collocation_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_level` (
  `id_collocation_level` int(11) NOT NULL AUTO_INCREMENT,
  `level` int(11) DEFAULT NULL,
  `game_type` int(11) DEFAULT NULL,
  `structure_id` int(11) DEFAULT NULL,
  `headword1` varchar(255) DEFAULT NULL,
  `headword2` varchar(255) DEFAULT NULL,
  `position` int(11) DEFAULT NULL,
  `active` int(1) DEFAULT '1',
  `deactivated` datetime DEFAULT NULL,
  `points_multiplier` int(11) DEFAULT '1',
  PRIMARY KEY (`id_collocation_level`),
  KEY `game_type_idx` (`game_type`),
  KEY `FK_structure_idx` (`structure_id`),
  CONSTRAINT `FK_game_type` FOREIGN KEY (`game_type`) REFERENCES `task_type` (`id`),
  CONSTRAINT `FK_structure` FOREIGN KEY (`structure_id`) REFERENCES `structure` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_level_title`
--

DROP TABLE IF EXISTS `collocation_level_title`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_level_title` (
  `level` int(11) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  `next_round` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_priority`
--

DROP TABLE IF EXISTS `collocation_priority`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_priority` (
  `collocation_id` int(11) NOT NULL,
  `priority` int(11) DEFAULT NULL,
  `specific_weight` int(11) DEFAULT NULL,
  `total_weight` int(11) DEFAULT NULL,
  `weight_limit` int(11) DEFAULT NULL,
  `game_type` int(11) NOT NULL,
  PRIMARY KEY (`collocation_id`,`game_type`),
  KEY `FK_game_type_col_priority_idx` (`game_type`),
  CONSTRAINT `FK_collocation` FOREIGN KEY (`collocation_id`) REFERENCES `collocation` (`id`),
  CONSTRAINT `FK_game_type_col_priority` FOREIGN KEY (`game_type`) REFERENCES `task_type` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_shell`
--

DROP TABLE IF EXISTS `collocation_shell`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_shell` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `structure_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `structure_id` (`structure_id`),
  CONSTRAINT `collocation_shell_ibfk_1` FOREIGN KEY (`structure_id`) REFERENCES `structure` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_shell_word`
--

DROP TABLE IF EXISTS `collocation_shell_word`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_shell_word` (
  `word_id` int(11) NOT NULL,
  `collocation_shell_id` int(11) NOT NULL,
  `position` int(11) NOT NULL,
  PRIMARY KEY (`word_id`,`collocation_shell_id`),
  KEY `collocation_shell_id` (`collocation_shell_id`),
  CONSTRAINT `collocation_shell_word_ibfk_1` FOREIGN KEY (`word_id`) REFERENCES `word` (`id`),
  CONSTRAINT `collocation_shell_word_ibfk_2` FOREIGN KEY (`collocation_shell_id`) REFERENCES `collocation_shell` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `collocation_word`
--

DROP TABLE IF EXISTS `collocation_word`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `collocation_word` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `word_id` int(11) NOT NULL,
  `collocation_id` int(11) NOT NULL,
  `position` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `collocation_id` (`collocation_id`),
  KEY `word_id` (`word_id`),
  CONSTRAINT `collocation_word_ibfk_1` FOREIGN KEY (`collocation_id`) REFERENCES `collocation` (`id`),
  CONSTRAINT `collocation_word_ibfk_2` FOREIGN KEY (`word_id`) REFERENCES `word` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `free_form`
--

DROP TABLE IF EXISTS `free_form`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `free_form` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `string` varchar(45) NOT NULL,
  `linguistic_unit_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `linguistic_unit_id` (`linguistic_unit_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `lexeme`
--

DROP TABLE IF EXISTS `lexeme`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lexeme` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `linguistic_unit`
--

DROP TABLE IF EXISTS `linguistic_unit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `linguistic_unit` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `possible_answer`
--

DROP TABLE IF EXISTS `possible_answer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `possible_answer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `linguistic_unit_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `linguistic_unit_id` (`linguistic_unit_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `structure`
--

DROP TABLE IF EXISTS `structure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `structure` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `headword_position` int(11) DEFAULT NULL,
  `text` varchar(80) NOT NULL DEFAULT '_',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `synonym`
--

DROP TABLE IF EXISTS `synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `synonym` (
  `linguistic_unit_id` int(11) NOT NULL,
  `linguistic_unit_id_syn` int(11) NOT NULL,
  `score` int(11) DEFAULT NULL,
  `difficulty` int(11) NOT NULL DEFAULT '0',
  `type` enum('core','near') DEFAULT NULL,
  `tid` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`linguistic_unit_id`,`linguistic_unit_id_syn`),
  KEY `synonym_ibfk_2_idx` (`linguistic_unit_id_syn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `synonym_log`
--

DROP TABLE IF EXISTS `synonym_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `synonym_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `level` int(11) DEFAULT NULL,
  `headword` varchar(255) DEFAULT NULL,
  `word1` varchar(255) DEFAULT NULL,
  `score1` int(11) DEFAULT NULL,
  `word2` varchar(255) DEFAULT NULL,
  `score2` int(11) DEFAULT NULL,
  `word3` varchar(255) DEFAULT NULL,
  `score3` int(11) DEFAULT NULL,
  `user` varchar(255) DEFAULT NULL,
  `created` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `synonym_unknown`
--

DROP TABLE IF EXISTS `synonym_unknown`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `synonym_unknown` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `headword` varchar(255) NOT NULL,
  `synonym` varchar(255) NOT NULL,
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `synonym_word`
--

DROP TABLE IF EXISTS `synonym_word`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `synonym_word` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `text` varchar(255) NOT NULL,
  `linguistic_unit_id` int(11) DEFAULT NULL,
  `lexeme_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `linguistic_unit_id` (`linguistic_unit_id`),
  KEY `lexeme_id` (`lexeme_id`),
  KEY `text` (`text`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task`
--

DROP TABLE IF EXISTS `task`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_cycle_id` int(11) NOT NULL,
  `collocation_shell_id` int(11) NOT NULL,
  `position` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `collocation_shell_id` (`collocation_shell_id`),
  KEY `task_cycle_id` (`task_cycle_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_answer`
--

DROP TABLE IF EXISTS `task_answer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task_answer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_user_id` int(11) NOT NULL,
  `ans_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `task_possible_answer_id` int(11) DEFAULT NULL,
  `position` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `task_user_id` (`task_user_id`),
  KEY `task_possible_answer_id` (`task_possible_answer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_cycle`
--

DROP TABLE IF EXISTS `task_cycle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task_cycle` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_type_id` int(11) NOT NULL,
  `thematic_id` int(11) DEFAULT NULL,
  `from_timestamp` timestamp NULL DEFAULT NULL,
  `to_timestamp` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `task_type_id` (`task_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_possible_answer`
--

DROP TABLE IF EXISTS `task_possible_answer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task_possible_answer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `possible_answer_id` int(11) NOT NULL,
  `task_id` int(11) NOT NULL,
  `score` int(11) DEFAULT NULL,
  `group_position` int(11) DEFAULT NULL,
  `choose_position` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `possible_answer_id` (`possible_answer_id`),
  KEY `task_id` (`task_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_type`
--

DROP TABLE IF EXISTS `task_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `title` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_user`
--

DROP TABLE IF EXISTS `task_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `task_user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_id` int(11) NOT NULL,
  `user_id` varchar(45) NOT NULL,
  `from_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `to_timestamp` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `task_id` (`task_id`),
  KEY `user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thematic`
--

DROP TABLE IF EXISTS `thematic`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thematic` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `task_type_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `task_type_id` (`task_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thematic_user`
--

DROP TABLE IF EXISTS `thematic_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thematic_user` (
  `thematic_id` int(11) NOT NULL,
  `user_id` varchar(45) NOT NULL,
  `thematic_score` int(11) NOT NULL DEFAULT '0',
  `thematic_position` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`thematic_id`,`user_id`),
  KEY `user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `uid` varchar(45) NOT NULL,
  `display_name` varchar(45) NOT NULL,
  `created` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `experience` int(11) NOT NULL DEFAULT '0',
  `choose_score` int(11) NOT NULL DEFAULT '0',
  `insert_score` int(11) NOT NULL DEFAULT '0',
  `drag_score` int(11) NOT NULL DEFAULT '0',
  `synonym_score` int(11) NOT NULL DEFAULT '0',
  `sum_score` int(11) NOT NULL DEFAULT '0',
  `campaign_score` int(11) NOT NULL DEFAULT '0',
  `campaign_level` int(11) NOT NULL DEFAULT '1',
  `age` varchar(45) DEFAULT NULL,
  `native_language` varchar(45) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `col_solo_score` int(11) DEFAULT '0',
  `co_solo_level` int(11) DEFAULT '1',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_col_level`
--

DROP TABLE IF EXISTS `user_col_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_col_level` (
  `user_id` varchar(45) NOT NULL,
  `collocation_level_id` int(11) NOT NULL,
  `type` enum('campaign','practice') NOT NULL,
  `score` int(11) DEFAULT NULL,
  `position` int(11) DEFAULT NULL,
  PRIMARY KEY (`user_id`,`collocation_level_id`,`type`),
  KEY `FK_collocation_level_idx` (`collocation_level_id`),
  CONSTRAINT `FK_collocation_level` FOREIGN KEY (`collocation_level_id`) REFERENCES `collocation_level` (`id_collocation_level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_level`
--

DROP TABLE IF EXISTS `user_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_level` (
  `id_user` varchar(45) NOT NULL,
  `level` int(11) NOT NULL,
  `linguistic_unit_id` int(11) NOT NULL,
  `score` int(11) DEFAULT '0',
  `position` int(11) DEFAULT '1',
  `type` enum('campaign','practice') NOT NULL DEFAULT 'practice',
  PRIMARY KEY (`id_user`,`level`,`linguistic_unit_id`,`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word`
--

DROP TABLE IF EXISTS `word`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `word` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `text` varchar(255) NOT NULL,
  `linguistic_unit_id` int(11) DEFAULT NULL,
  `lexeme_id` int(11) DEFAULT NULL,
  `variants` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `linguistic_unit_id` (`linguistic_unit_id`),
  KEY `lexeme_id` (`lexeme_id`),
  KEY `text` (`text`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-01-31 16:28:21

INSERT INTO task_type (id, name) VALUES (1, "choose");
INSERT INTO task_type (id, name) VALUES (2, "insert");
INSERT INTO task_type (id, name) VALUES (3, "drag");
INSERT INTO task_type (id, name) VALUES (4, "thematic");
INSERT INTO task_type (id, name) VALUES (5, "synonym");
