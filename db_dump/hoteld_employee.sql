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
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `idemployee` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `lastname` varchar(50) NOT NULL,
  `patronymic` varchar(50) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `birth` date NOT NULL,
  `password` varchar(50) NOT NULL,
  `phone_number` varchar(11) NOT NULL,
  `idrole` int NOT NULL,
  `idhotel` int DEFAULT NULL,
  `status` tinyint NOT NULL,
  PRIMARY KEY (`idemployee`),
  KEY `idrole_idx` (`idrole`),
  KEY `idhotel_idx` (`idhotel`),
  CONSTRAINT `idhotel` FOREIGN KEY (`idhotel`) REFERENCES `hotel` (`idhotel`),
  CONSTRAINT `idrole` FOREIGN KEY (`idrole`) REFERENCES `role` (`idrole`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES (1,'Карина','Ишмухаметова','Айдаровна','karina.ishmuhametova@yandex.ru','2005-12-24','1','89279306308',1,1,0),(2,'Кирилл','Маркин','Дамирович','kirill.markin@yandex.ru','2000-01-01','2','89277894512',2,1,0),(3,'Ярослав','Фролов','Арсентьевич','slava.Frolov@yandex.ru','2000-01-01','3','89214561223',2,2,0),(4,'Кристина','Михайлова','Андреевна','kriss.mix@yandex.ru','2000-02-02','4','84562137845',3,1,0),(5,'Марк','Григорьев','Романович','markmark@yandex.ru','1999-01-05','5','89274561232',2,3,0),(6,'Павел','Иванов','Иванович','ivanivan@gmail.com','1990-07-18','123456789','89234321234',2,2,0),(8,'Иван','Сергеев','Денисович','ivanSergeev@gmail.com','1999-05-05','654321','89213211321',3,2,0),(9,'Анита','Ханова','Айратовна','anita@gmail.com','2002-03-08','654321','89213284545',3,2,0);
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-10 11:40:06
