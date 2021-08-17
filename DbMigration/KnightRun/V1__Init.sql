-- --------------------------------------------------------
-- 호스트:                          127.0.0.1
-- 서버 버전:                        10.3.7-MariaDB - mariadb.org binary distribution
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 테이블 knightrun.accounts 구조 내보내기
CREATE TABLE IF NOT EXISTS `accounts` (
  `userId` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `nickName` varchar(20) NOT NULL,
  PRIMARY KEY (`userId`),
  UNIQUE KEY `UX_accounts_nickName` (`nickName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.accounts_social 구조 내보내기
CREATE TABLE IF NOT EXISTS `accounts_social` (
  `userId` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `nickName` varchar(20) NOT NULL,
  `deviceId` varchar(20) NOT NULL,
  `socialType` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `socialId` varchar(30) NOT NULL,
  PRIMARY KEY (`userId`),
  UNIQUE KEY `UX_accounts_nickName` (`nickName`),
  UNIQUE KEY `UX_accounts_deviceId` (`deviceId`),
  KEY `IX_accounts_nickName` (`nickName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.cards 구조 내보내기
CREATE TABLE IF NOT EXISTS `cards` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `cardId` int(11) unsigned NOT NULL,
  `level` int(11) unsigned NOT NULL DEFAULT 0,
  `count` int(11) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_cards_userId_cardId` (`userId`,`cardId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.decks 구조 내보내기
CREATE TABLE IF NOT EXISTS `decks` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `modeType` tinyint(4) unsigned NOT NULL DEFAULT 0,
  `deckType` tinyint(4) unsigned NOT NULL DEFAULT 0,
  `deckNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `orderNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `classId` int(11) unsigned NOT NULL,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_decks_userId_modeType_deckNum_deckType_orderNum` (`userId`,`modeType`,`deckType`,`deckNum`,`orderNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Deck Info';

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.equipments 구조 내보내기
CREATE TABLE IF NOT EXISTS `equipments` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `equipId` int(11) unsigned NOT NULL,
  `level` int(11) unsigned NOT NULL DEFAULT 0,
  `count` int(11) unsigned NOT NULL DEFAULT 0,
  `unitId` int(11) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_equipments_equipId_userId` (`userId`,`equipId`),
  KEY `IX_equipments_unitId` (`unitId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.move_device 구조 내보내기
CREATE TABLE IF NOT EXISTS `move_device` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL DEFAULT 0,
  `socialType` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `socialId` varchar(30) NOT NULL,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_move_device_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 테이블 knightrun.stage_levels 구조 내보내기
CREATE TABLE IF NOT EXISTS `stage_levels` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL DEFAULT 0,
  `stageLevelId` int(11) unsigned NOT NULL DEFAULT 0,
  `clearCount` int(11) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_stage_levels_stageLevelId_userId` (`userId`,`stageLevelId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.units 구조 내보내기
CREATE TABLE IF NOT EXISTS `units` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `unitId` int(11) unsigned NOT NULL,
  `level` int(11) unsigned NOT NULL DEFAULT 0,
  `count` int(11) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_units_userId_unitId` (`userId`,`unitId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 테이블 knightrun.users 구조 내보내기
CREATE TABLE IF NOT EXISTS `users` (
  `seq` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `gold` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_users_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 프로시저 knightrun.p_equipmentPutOff 구조 내보내기
-- Creation

DELIMITER $$
CREATE PROCEDURE `p_equipmentPutOff`(
	IN `$userId` INT UNSIGNED,
	IN `$equipId` INT UNSIGNED
)
BEGIN  
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;  	

	SELECT COUNT(userId) INTO @count FROM equipments WHERE userId = $userId AND equipId = $equipId;
	IF @count = 0 THEN
		SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 6, MESSAGE_TEXT = 'Invalid equipId';
	END IF;

	UPDATE equipments SET unitId=0 WHERE userId=$userId AND equipId=$equipId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `p_equipmentPutOn`(
	IN `$userId` INT UNSIGNED,
	IN `$equipId` INT UNSIGNED,
	IN `$unitId` INT UNSIGNED
)
BEGIN  
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;  	
	
	SELECT COUNT(userId) INTO @count FROM equipments WHERE userId = $userId AND equipId = $equipId;
	IF @count = 0 THEN
		SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 6, MESSAGE_TEXT = 'Invalid equipId';
	END IF;
	
	UPDATE equipments SET unitId=$unitId WHERE userId=$userId AND equipId=$equipId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	OUT `$isFirstMember` BIT

)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
	SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName) VALUES($nickName);
		SET @userId = LAST_INSERT_ID();
		SET $isFirstMember = 1;

		SELECT userId FROM accounts WHERE userId = @userId;
	ELSE 
		SET $isFirstMember = 0;		
		SELECT userId FROM accounts WHERE nickName = $nickName;
	END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `p_getClearCountAfterStageLevelWin`(
	IN `$userId` INT,
	IN `$stageLevelId` INT

)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	INSERT INTO stage_levels (userId, stageLevelId, clearCount) VALUES ($userId, $stageLevelId, 1)
	ON DUPLICATE KEY UPDATE clearCount = clearCount + 1;
	SELECT clearCount FROM stage_levels WHERE userId = $userId AND stageLevelId=$stageLevelId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT





)
BEGIN
      DECLARE EXIT HANDLER FOR SQLEXCEPTION      
      BEGIN
          ROLLBACK;
          
          GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
          
        SET @text = CONCAT(@sqlstate, "|", @text);
        SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
      END;

    SELECT gold FROM users WHERE userId = $userId;
    SELECT cardId, level, count FROM cards WHERE userId = $userId;
    SELECT unitId, level, count FROM units WHERE userId = $userId;
    SELECT rankType, rankPoint, rankTierMatchingId FROM ranks WHERE userId = $userId;
    SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `p_setDeck`(
	IN `$userId` INT,
	IN `$modeType` TINYINT,
	IN `$deckType` TINYINT,
	IN `$deckNum` TINYINT,
	IN `$orderNum` TINYINT,
	IN `$classId` INT


















)
BEGIN  
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
  	START TRANSACTION;
	  	
	/*
	IF $deckType = 1 THEN #character
		SELECT COUNT(unitId) INTO @unitId FROM units WHERE userId = $userId AND unitId = $classId;
		IF @unitId = 0 THEN
			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 6, MESSAGE_TEXT = 'character InvalidTargetId';
		END IF;
	ELSEIF $deckType = 2 THEN #card
		SELECT COUNT(cardId) INTO @cardId FROM cards WHERE userId = $userId AND cardId = $classId;
		IF @cardId = 0 THEN
			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 6, MESSAGE_TEXT = 'card InvalidTargetId';
		END IF;
	END IF;*/
  	
	INSERT INTO decks(userId, modeType, deckType, deckNum, orderNum, classId) 
	VALUES($userId, $modeType, $deckType, $deckNum, $orderNum, $classId)
	ON DUPLICATE KEY UPDATE classId = $classId;
	
	COMMIT;
END$$
DELIMITER ;