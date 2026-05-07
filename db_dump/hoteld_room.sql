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
-- Table structure for table `room`
--

DROP TABLE IF EXISTS `room`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `room` (
  `idroom` int NOT NULL AUTO_INCREMENT,
  `name` varchar(10) NOT NULL,
  `floorid` int NOT NULL,
  `id_category` int NOT NULL,
  `status_id` int NOT NULL,
  `hotelid` int NOT NULL,
  PRIMARY KEY (`idroom`),
  KEY `floorid_idx` (`floorid`),
  KEY `id_category_idx` (`id_category`),
  KEY `status_id_idx` (`status_id`),
  KEY `hotelid_idx` (`hotelid`),
  CONSTRAINT `floorid` FOREIGN KEY (`floorid`) REFERENCES `floor` (`idfloor`),
  CONSTRAINT `hotelid` FOREIGN KEY (`hotelid`) REFERENCES `hotel` (`idhotel`),
  CONSTRAINT `id_category` FOREIGN KEY (`id_category`) REFERENCES `category` (`idcategory`),
  CONSTRAINT `status_id` FOREIGN KEY (`status_id`) REFERENCES `status_room` (`idstatus_room`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `room`
--

LOCK TABLES `room` WRITE;
/*!40000 ALTER TABLE `room` DISABLE KEYS */;
INSERT INTO `room` VALUES (1,'311',3,1,2,1),(2,'102',1,3,1,1),(3,'103',1,1,1,1),(4,'104',1,2,1,1),(5,'201',2,1,1,1),(10,'306',3,3,2,1),(11,'305',3,5,2,1),(12,'406',4,2,2,1),(13,'67',6,4,2,1),(14,'301',3,1,2,1),(15,'676',6,3,2,1),(16,'101',1,1,2,2),(17,'102',1,2,2,2),(18,'201',2,1,2,2);
/*!40000 ALTER TABLE `room` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-07 18:36:03
