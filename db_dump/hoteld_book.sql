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
-- Table structure for table `book`
--

DROP TABLE IF EXISTS `book`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `book` (
  `idbook` int NOT NULL AUTO_INCREMENT,
  `employee_id` int DEFAULT NULL,
  `room_id` int NOT NULL,
  `check_in_date` date NOT NULL,
  `departure_date` date NOT NULL,
  `booking_date` date NOT NULL,
  `payment` tinyint NOT NULL,
  `status_book` tinyint NOT NULL,
  `client_id` int NOT NULL,
  PRIMARY KEY (`idbook`),
  KEY `employee_id_idx` (`employee_id`),
  KEY `room_id_idx` (`room_id`),
  KEY `client_id_idx` (`client_id`),
  CONSTRAINT `client_id` FOREIGN KEY (`client_id`) REFERENCES `clint` (`idclint`),
  CONSTRAINT `employee_id` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`idemployee`),
  CONSTRAINT `room_id` FOREIGN KEY (`room_id`) REFERENCES `room` (`idroom`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `book`
--

LOCK TABLES `book` WRITE;
/*!40000 ALTER TABLE `book` DISABLE KEYS */;
INSERT INTO `book` VALUES (1,4,1,'2025-12-08','2025-12-15','2025-12-01',1,3,1),(2,4,2,'2025-12-10','2025-12-12','2025-12-03',1,3,1),(3,4,1,'2025-12-22','2025-12-25','2025-12-15',1,3,1),(33,4,1,'2026-04-27','2026-04-30','2026-04-05',1,3,8),(34,4,11,'2026-04-07','2026-05-07','2026-04-06',1,3,13),(35,4,11,'2026-05-08','2026-05-15','2026-04-06',1,1,4),(36,4,4,'2026-04-11','2026-04-15','2026-04-10',1,3,8),(37,4,10,'2026-05-04','2026-05-08','2026-04-10',1,3,3),(39,4,1,'2026-04-11','2026-04-15','2026-04-10',1,3,2),(40,4,3,'2026-04-22','2026-04-30','2026-04-10',1,3,4),(41,4,5,'2026-04-20','2026-04-30','2026-04-10',1,3,12),(42,4,10,'2026-06-01','2026-06-03','2026-04-10',1,1,3),(43,4,5,'2026-05-18','2026-05-31','2026-04-14',1,1,14),(45,4,5,'2026-05-03','2026-05-07','2026-05-03',1,3,14),(46,4,10,'2026-05-08','2026-05-12','2026-05-03',1,1,14),(47,1,1,'2026-05-07','2026-05-13','2026-05-05',1,1,1),(48,1,1,'2026-06-01','2026-06-04','2026-05-05',1,1,1),(49,1,1,'2026-07-01','2026-07-11','2026-05-05',1,1,1),(50,6,18,'2026-07-01','2026-07-11','2026-05-07',1,1,1);
/*!40000 ALTER TABLE `book` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-10 11:40:05
