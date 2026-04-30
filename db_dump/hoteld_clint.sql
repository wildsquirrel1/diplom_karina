-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: hoteld
-- ------------------------------------------------------
-- Server version	8.0.37

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
-- Table structure for table `clint`
--

DROP TABLE IF EXISTS `clint`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clint` (
  `idclint` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `lastname` varchar(50) NOT NULL,
  `patronymic` varchar(50) NOT NULL,
  `seria_pass` varchar(4) NOT NULL,
  `number_pass` varchar(6) NOT NULL,
  `birth` date NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(50) NOT NULL,
  `photo` longblob,
  PRIMARY KEY (`idclint`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clint`
--

LOCK TABLES `clint` WRITE;
/*!40000 ALTER TABLE `clint` DISABLE KEYS */;
INSERT INTO `clint` VALUES (1,'Алена','Иванова','','8020','123456','2000-01-01','sadfeeee@gmail.com','123456',NULL),(2,'Оливия','Савина','Дмитриревна','5412','321654','1997-04-10','OliviaSav@yandex.ru','654321',NULL),(3,'Кристина','Киселова','Артемовна','4565','789456','1998-01-12','KrissKiss@yandex.ru','123123',NULL),(4,'Иван','Иванов','Иванович','8020','123456','2002-08-15','rewq@gmail.com','654321',NULL),(7,'Елена','Иванова','','8020','123211','2019-01-03','ivanovaelena@gmail.com','654321',NULL),(8,'Марат','Маратов','Маратович','4015','315741','2020-07-10','marat@gmail.com','654321',NULL),(9,'Алена','Иванова','Александрова','5412','103456','1991-07-10','alenaivanova@gmail.com','654321',NULL),(11,'Петр','Петров','string','8020','942674','2000-01-01','petr@gmail.com','123456789',NULL),(12,'Камила','Ишмухаметова','Айдаровна','8025','122332','2005-12-24','kamila@gmail.com','123456',NULL),(13,'Лиана','Набиева','Рамилевна','1234','567890','2006-03-09','liana@gmail.com','1234567',NULL),(14,'Платон','Пахомов','Иванович','8021','563412','1991-07-10','Platon-Pah@yandex.ru','654321',NULL),(15,'Карина','Ишмухаметова','','8012','122121','2004-03-02','karina@yandex.ru','123456',NULL);
/*!40000 ALTER TABLE `clint` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-29 18:56:37
