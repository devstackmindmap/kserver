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

-- 테이블 kpi.daily_buy_digital 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_buy_digital` (
  `productId` int(11) DEFAULT NULL,
  `materialType` int(11) DEFAULT NULL,
  `totalcost` int(11) DEFAULT NULL,
  `cost` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `productId_materialType_date` (`productId`,`materialType`,`date`),
  KEY `date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_buy_real 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_buy_real` (
  `productId` int(11) DEFAULT NULL,
  `platformType` int(11) DEFAULT NULL,
  `totalcost` int(11) DEFAULT NULL,
  `cost` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `productId_materialType_date` (`productId`,`platformType`,`date`),
  KEY `date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_infusionbox_item 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_infusionbox_item` (
  `boxId` int(11) DEFAULT NULL,
  `boxOpenType` int(11) DEFAULT NULL,
  `ItemCode` int(11) DEFAULT NULL,
  `amount` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `Date` date DEFAULT NULL,
  UNIQUE KEY `boxId_boxOpenType_ItemCode_Date` (`boxId`,`boxOpenType`,`ItemCode`,`Date`),
  KEY `Date` (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='인퓨전 박스에서 오픈되어 해금된 스킬 혹은 캐릭터 정보';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_itemembargo 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_itemembargo` (
  `boxId` int(11) DEFAULT NULL,
  `boxOpenType` int(11) DEFAULT NULL,
  `ItemCode` int(11) DEFAULT NULL,
  `removeEmbargoCount` int(11) DEFAULT NULL,
  `Date` date DEFAULT NULL,
  UNIQUE KEY `boxId_boxOpenType_ItemCode_Date` (`boxId`,`boxOpenType`,`ItemCode`,`Date`),
  KEY `Date` (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='인퓨전 박스에서 오픈되어 해금된 스킬 혹은 캐릭터 정보';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_item_etc_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_item_etc_log` (
  `LogType` varchar(20) DEFAULT NULL,
  `ClassId` int(11) DEFAULT NULL,
  `Category` varchar(20) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `LogType_ClassId_Category_date` (`LogType`,`ClassId`,`Category`,`date`),
  KEY `date` (`date`),
  KEY `LogType` (`LogType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='당일 스킨 획득, 이모티콘 획득, 컨텐츠 획득, 유저프로파일 획득 로그...';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_item_material_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_item_material_log` (
  `logType` tinyint(4) DEFAULT NULL COMMENT '1 : get 2: use',
  `materialType` char(15) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `callcount` int(11) DEFAULT NULL COMMENT '로그 남은 횟수',
  `category` varchar(20) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `logType_materialType_category_date` (`logType`,`materialType`,`category`,`date`),
  KEY `date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='아이템 로그에서 piece Get 과  use 관련 데이터 데일리 로그';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_item_piece_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_item_piece_log` (
  `logType` tinyint(4) DEFAULT NULL COMMENT '1 : get 2: use',
  `tableName` char(15) DEFAULT NULL,
  `classId` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `callcount` int(11) DEFAULT NULL COMMENT '로그 남은 횟수',
  `category` varchar(20) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `logType_tableName_classId_category_date` (`logType`,`tableName`,`classId`,`category`,`date`),
  KEY `date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='아이템 로그에서 piece Get 과  use 관련 데이터 데일리 로그';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_matchlog 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_matchlog` (
  `BattleType` int(11) DEFAULT NULL,
  `EnemyType` varchar(10) DEFAULT NULL,
  `RankTire` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  UNIQUE KEY `BattleType_EnemyType_RankTire_date` (`BattleType`,`EnemyType`,`RankTire`,`date`),
  KEY `date` (`date`),
  KEY `Rankpoint` (`RankTire`),
  KEY `BattleType` (`BattleType`),
  KEY `EnemyType` (`EnemyType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_migretion 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_migretion` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `date` date DEFAULT NULL COMMENT '날짜',
  `boosterBattleType_1_count` int(11) DEFAULT NULL COMMENT 'PVP 부스트타임 시작 카운트',
  `boosterBattleType_2_count` int(11) DEFAULT NULL COMMENT 'PVE 부스트타임 시작 카운트',
  `extensionTimeBattleType_1_count` int(11) DEFAULT NULL COMMENT 'PVP 연장 시작 카운트',
  `extensionTimeBattleType_2_count` int(11) DEFAULT NULL COMMENT 'PVE 연장 시작 카운트',
  `enterRoomBattleType_1_count` int(11) DEFAULT NULL COMMENT 'PVP 입장 카운트',
  `enterRoomBattleType_2_count` int(11) DEFAULT NULL COMMENT 'PVE 입장 카운트',
  `reEnterRoom_count` int(11) DEFAULT NULL COMMENT '재입장 카운트',
  `retreatBattleType_1_count` int(11) DEFAULT NULL COMMENT 'PVP 후퇴 카운트',
  `retreatBattleType_2_count` int(11) DEFAULT NULL COMMENT 'PVE 후퇴 카운트',
  PRIMARY KEY (`seq`),
  KEY `ix_date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.daily_open_infusionbox_count 구조 내보내기
CREATE TABLE IF NOT EXISTS `daily_open_infusionbox_count` (
  `boxId` int(11) NOT NULL DEFAULT 0,
  `count` int(11) NOT NULL DEFAULT 0,
  `Date` date NOT NULL,
  UNIQUE KEY `boxId_Date` (`boxId`,`Date`),
  KEY `Date` (`Date`),
  KEY `boxId` (`boxId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.dospell_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `dospell_log` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `unitId` int(11) DEFAULT NULL,
  `targetUnitId` int(11) DEFAULT NULL,
  `cardId` int(11) DEFAULT NULL,
  `skillId` int(11) DEFAULT NULL,
  `totalHP` bigint(20) DEFAULT NULL,
  `totalPrvHP` bigint(20) DEFAULT NULL,
  `avgDamage` int(11) DEFAULT NULL,
  `maxDamage` int(11) DEFAULT NULL,
  `MinDamage` int(11) DEFAULT NULL,
  `count` bigint(20) DEFAULT NULL,
  `DATE` date DEFAULT NULL,
  PRIMARY KEY (`seq`),
  KEY `DATE` (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.enter_room 구조 내보내기
CREATE TABLE IF NOT EXISTS `enter_room` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `BattleType` smallint(6) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`seq`),
  KEY `date` (`date`),
  KEY `BattleType` (`BattleType`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.login_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `login_log` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`seq`),
  KEY `_ix_date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.normalattack 구조 내보내기
CREATE TABLE IF NOT EXISTS `normalattack` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `unitId` int(11) NOT NULL DEFAULT 0,
  `isCritical` bit(1) NOT NULL DEFAULT b'0',
  `totalDamage` bigint(20) NOT NULL DEFAULT 0,
  `attackCount` int(11) NOT NULL DEFAULT 0,
  `avgDamage` int(11) NOT NULL DEFAULT 0,
  `date` date NOT NULL,
  PRIMARY KEY (`seq`),
  KEY `ix_date` (`date`),
  KEY `ix_unitId` (`unitId`)
) ENGINE=InnoDB AUTO_INCREMENT=2797 DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.reenter_room 구조 내보내기
CREATE TABLE IF NOT EXISTS `reenter_room` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `count` int(11) NOT NULL DEFAULT 0,
  `date` date NOT NULL,
  PRIMARY KEY (`seq`),
  KEY `date` (`date`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.retreat 구조 내보내기
CREATE TABLE IF NOT EXISTS `retreat` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `BattleType` smallint(6) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`seq`),
  KEY `date` (`date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 kpi.usecardhistory 구조 내보내기
CREATE TABLE IF NOT EXISTS `usecardhistory` (
  `seq` int(11) NOT NULL AUTO_INCREMENT,
  `battleType` int(11) NOT NULL DEFAULT 0 COMMENT 'pvp 1, pve 2, tutorial 14',
  `usedCardId` int(11) NOT NULL DEFAULT 0 COMMENT '사용한 카드 ID',
  `resultUse` char(10) NOT NULL DEFAULT '0' COMMENT '성공 실패',
  `count` int(11) NOT NULL DEFAULT 0 COMMENT '사용횟수',
  `date` date DEFAULT NULL COMMENT '날자',
  PRIMARY KEY (`seq`),
  KEY `Ix_date` (`date`),
  KEY `Ix_UsedCardId` (`usedCardId`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
