-- --------------------------------------------------------
-- 호스트:                          47.74.6.156
-- 서버 버전:                        10.3.7-MariaDB-1:10.3.7+maria~jessie - mariadb.org binary distribution
-- 서버 OS:                        debian-linux-gnu
-- HeidiSQL 버전:                  9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 테이블 knightrun._common_cardName 구조 내보내기
CREATE TABLE IF NOT EXISTS `_common_cardName` (
  `sn` int(11) NOT NULL AUTO_INCREMENT,
  `cardId` int(11) NOT NULL,
  `cardName` varchar(30) NOT NULL,
  PRIMARY KEY (`sn`),
  KEY `cardId` (`cardId`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8;

-- 테이블 데이터 knightrun._common_cardName:~43 rows (대략적) 내보내기
DELETE FROM `_common_cardName`;
/*!40000 ALTER TABLE `_common_cardName` DISABLE KEYS */;
INSERT INTO `_common_cardName` (`sn`, `cardId`, `cardName`) VALUES
	(1, 103, '대함도'),
	(2, 104, '문버스터'),
	(3, 105, 'A시스템 가동'),
	(4, 106, '엔젤실드'),
	(5, 107, '동력전환'),
	(6, 108, '강화 장갑 부여'),
	(7, 201, '나선청파기공 강진선파'),
	(8, 203, '무명이검 전력베기'),
	(9, 204, '압도적인 적의'),
	(10, 205, '성괴붕권'),
	(11, 206, '무명일검 무기깨기'),
	(12, 207, '필연적 승리'),
	(13, 208, '공격 궤도 예측'),
	(14, 301, '적색정점 태양검'),
	(15, 303, '디비전 셀 오버부스트'),
	(16, 304, '현월 적'),
	(17, 305, '전술 어드바이저'),
	(18, 306, '사상정보체 발현'),
	(19, 307, '무명 종베기'),
	(20, 308, '적파기공'),
	(21, 401, '그린링코어'),
	(22, 403, '평화의 찬가'),
	(23, 404, '다중 연속 지르기'),
	(24, 405, '일점 지르기'),
	(25, 406, '집중'),
	(26, 407, '강인한 의지'),
	(27, 408, '과감한 공세'),
	(28, 501, '바리사다 사출A'),
	(29, 503, '울부짓는 포효'),
	(30, 504, '분쇄'),
	(31, 505, '지면 휩쓸기'),
	(32, 506, '뚜렷한 악의'),
	(33, 507, '다중 침식'),
	(34, 508, '결계장'),
	(35, 601, '바리사다 사출B'),
	(36, 603, '사상력 극대화'),
	(37, 604, '끊임없는 추격'),
	(38, 605, '파국의 전조'),
	(39, 606, '절망의 순간'),
	(40, 607, '자밀기관 전개'),
	(41, 608, '방사능 확산'),
	(42, 9910001, '적군 전체 즉사'),
	(43, 9910002, '아군 전체 즉사');
/*!40000 ALTER TABLE `_common_cardName` ENABLE KEYS */;

-- 테이블 knightrun._common_equipName 구조 내보내기
CREATE TABLE IF NOT EXISTS `_common_equipName` (
  `sn` int(11) NOT NULL AUTO_INCREMENT,
  `Id` int(11) NOT NULL,
  `Name` varchar(30) NOT NULL,
  PRIMARY KEY (`sn`),
  KEY `Id` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- 테이블 데이터 knightrun._common_equipName:~12 rows (대략적) 내보내기
DELETE FROM `_common_equipName`;
/*!40000 ALTER TABLE `_common_equipName` DISABLE KEYS */;
INSERT INTO `_common_equipName` (`sn`, `Id`, `Name`) VALUES
	(1, 11001, '1번검 더 원'),
	(2, 12001, '5번검 V'),
	(3, 13001, '15번검 그린링'),
	(4, 14001, 'M4소드 레벨 2 혈검 아누비스'),
	(5, 15001, '사상무기 골드 미라쥬'),
	(6, 16001, '680번검 아머드블레이드'),
	(7, 21001, 'DC 코트 NO.1'),
	(8, 22001, 'DC 코트 NO.2'),
	(9, 23001, 'DC 코트 NO.3'),
	(10, 24001, 'DC 코트 NO.4'),
	(11, 25001, 'DC 코트 NO.5'),
	(12, 26001, 'DC 코트 NO.6');
/*!40000 ALTER TABLE `_common_equipName` ENABLE KEYS */;

-- 테이블 knightrun._common_unitName 구조 내보내기
CREATE TABLE IF NOT EXISTS `_common_unitName` (
  `sn` int(11) NOT NULL AUTO_INCREMENT,
  `unitId` int(11) NOT NULL,
  `unitName` varchar(30) NOT NULL,
  PRIMARY KEY (`sn`),
  KEY `unitId` (`unitId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- 테이블 데이터 knightrun._common_unitName:~8 rows (대략적) 내보내기
DELETE FROM `_common_unitName`;
/*!40000 ALTER TABLE `_common_unitName` DISABLE KEYS */;
INSERT INTO `_common_unitName` (`sn`, `unitId`, `unitName`) VALUES
	(1, 1001, 'A10'),
	(2, 1002, 'Pray'),
	(3, 1003, 'Anne Mayer'),
	(4, 1004, 'Mei Xile'),
	(5, 1005, 'Cross Eye Alpha'),
	(6, 1006, 'Cross Eye Beta'),
	(7, 1007, 'Leo'),
	(8, 1008, 'Daniel');
/*!40000 ALTER TABLE `_common_unitName` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
