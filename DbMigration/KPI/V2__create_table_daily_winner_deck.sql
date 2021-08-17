-- --------------------------------------------------------
-- 호스트:                          127.0.0.1
-- 서버 버전:                        10.3.10-MariaDB - mariadb.org binary distribution
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 테이블 kpi.daily_winners_deck 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_winners_deck` (
  `battleType` int(11) DEFAULT NULL,
  `resultType` int(11) DEFAULT NULL COMMENT '1 win/lose 2 draw',
  `winnerUnits` varchar(50) DEFAULT NULL,
  `winnerUnitLevels` varchar(20) DEFAULT NULL,
  `winnercards` varchar(50) DEFAULT NULL,
  `loseUnits` varchar(50) DEFAULT NULL,
  `loseUnitLevels` varchar(20) DEFAULT NULL,
  `losecards` varchar(50) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `avgPlayTime` int(11) DEFAULT NULL,
  `maxPlayTime` int(11) DEFAULT NULL,
  `minPlayTime` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  KEY `date` (`date`),
  KEY `battleType` (`battleType`),
  KEY `resultType` (`resultType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='전투 결과 승자와 덱 구성 정보.';

-- 내보낼 데이터가 선택되어 있지 않습니다.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
