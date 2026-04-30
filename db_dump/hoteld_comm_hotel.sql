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
-- Table structure for table `comm_hotel`
--

DROP TABLE IF EXISTS `comm_hotel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comm_hotel` (
  `idcomm_hotel` int NOT NULL AUTO_INCREMENT,
  `id_client` int NOT NULL,
  `id_hotel` int NOT NULL,
  `id_comm` int NOT NULL,
  PRIMARY KEY (`idcomm_hotel`),
  KEY `commid_idx` (`id_comm`),
  KEY `hotel_id_idx` (`id_hotel`),
  KEY `clintidd_idx` (`id_client`),
  CONSTRAINT `clintidd` FOREIGN KEY (`id_client`) REFERENCES `clint` (`idclint`),
  CONSTRAINT `commid` FOREIGN KEY (`id_comm`) REFERENCES `comment` (`idcomment`),
  CONSTRAINT `hotel_id` FOREIGN KEY (`id_hotel`) REFERENCES `hotel` (`idhotel`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comm_hotel`
--

LOCK TABLES `comm_hotel` WRITE;
/*!40000 ALTER TABLE `comm_hotel` DISABLE KEYS */;
INSERT INTO `comm_hotel` VALUES (1,1,1,1),(2,1,2,2),(3,2,3,3),(4,3,4,6);
/*!40000 ALTER TABLE `comm_hotel` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-29 18:56:36
