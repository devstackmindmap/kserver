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

-- 테이블 knightrun_gmtool.game_table 구조 내보내기
CREATE TABLE IF NOT EXISTS `game_table` (
  `version` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `tableType` varchar(50) NOT NULL,
  `runMode` enum('Dev1','Dev2','QA','Live','Local') NOT NULL,
  `url` varchar(150) NOT NULL,
  `comment` text NOT NULL,
  `createDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`version`),
  KEY `IX_gameTable_tableType_serverType` (`tableType`,`runMode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
-- 프로시저 knightrun_gmtool.p_getGameTableBetweenVersion 구조 내보내기
DELIMITER //
CREATE PROCEDURE `p_getGameTableBetweenVersion`(
	IN `$fromVersion` BIGINT,
	IN `$toVersion` BIGINT,
	IN `$runMode` VARCHAR(10)




)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		IF @sqlstate = '45000' THEN
 			SIGNAL SQLSTATE '45000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
 		ELSE
 			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 200, MESSAGE_TEXT = @text;
  		END IF;
  	END;

	SELECT max(version) as version, max(url) as url, tableType
	FROM game_table WHERE version > $fromVersion and version <= $toVersion and runMode = $runMode GROUP BY tableType;
END//
DELIMITER ;

-- 프로시저 knightrun_gmtool.p_getGameTableLatestVersion 구조 내보내기
DELIMITER //
CREATE PROCEDURE `p_getGameTableLatestVersion`(
	IN `$version` BIGINT,
	IN `$runMode` VARCHAR(10)

















)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
  		IF @sqlstate = '45000' THEN
 			SIGNAL SQLSTATE '45000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
 		ELSE
 			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 200, MESSAGE_TEXT = @text;
  		END IF;
  	END;

	SELECT max(version) as version, max(url) as url, tableType
	FROM game_table WHERE version > $version and runMode = $runMode GROUP BY tableType;
END//
DELIMITER ;

-- 프로시저 knightrun_gmtool.p_getGameTableWithinVersion 구조 내보내기
DELIMITER //
CREATE PROCEDURE `p_getGameTableWithinVersion`(
	IN `$version` BIGINT,
	IN `$runMode` VARCHAR(10)












)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		IF @sqlstate = '45000' THEN
 			SIGNAL SQLSTATE '45000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
 		ELSE
 			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 200, MESSAGE_TEXT = @text;
  		END IF;
  	END;

	SELECT max(version) as version, max(url) as url, tableType
	FROM game_table WHERE version <= $version and runMode = $runMode GROUP BY tableType;
END//
DELIMITER ;

-- 프로시저 knightrun_gmtool.p_transferLatestGameTable 구조 내보내기
DELIMITER //
CREATE PROCEDURE `p_transferLatestGameTable`(
	IN `$from_runMode` VARCHAR(10),
	IN `$to_runMode` VARCHAR(10)










)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
  		IF @sqlstate = '45000' THEN
 			SIGNAL SQLSTATE '45000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
 		ELSE
 			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 200, MESSAGE_TEXT = @text;
  		END IF;
  	END;

	START TRANSACTION;
		INSERT INTO game_table(tableType, url, runMode, comment, createDate) 	
		SELECT to_runMode.tableType, to_runMode.url, $to_runMode as runMode, to_runMode.comment, to_runMode.createDate FROM (
			SELECT tableType, max(version) as version, comment, createDate FROM game_table WHERE runMode = $from_runMode GROUP BY tableType
		) from_runMode INNER JOIN game_table to_runMode ON from_runMode.tableType = to_runMode.tableType and from_runMode.version = to_runMode.version;
	COMMIT;
END//
DELIMITER ;

-- 프로시저 knightrun_gmtool.p_updateGameTableToServer 구조 내보내기
DELIMITER //
CREATE PROCEDURE `p_updateGameTableToServer`(
	IN `$hostName` VARCHAR(50),
	IN `$ip` VARCHAR(30),
	IN `$tableVersion` BIGINT,
	IN `$runMode` VARCHAR(10),
	IN `$serverName` VARCHAR(30),
	IN `$updateTime` TIMESTAMP




)
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
  		IF @sqlstate = '45000' THEN
 			SIGNAL SQLSTATE '45000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
 		ELSE
 			SIGNAL SQLSTATE '46000' SET MYSQL_ERRNO = 200, MESSAGE_TEXT = @text;
  		END IF;
  	END;
  	
  	START TRANSACTION;
  		INSERT INTO server_list(ip, hostName, tableVersion, runMode, serverName, updateTime) VALUES($ip, $hostName, $tableVersion, $runMode, $serverName, $updateTime)
  		ON DUPLICATE KEY UPDATE tableVersion = $tableVersion, updateTime = $updateTime;
  	COMMIT;
END//
DELIMITER ;

-- 테이블 knightrun_gmtool.server_list 구조 내보내기
CREATE TABLE IF NOT EXISTS `server_list` (
  `ip` varchar(30) NOT NULL,
  `runMode` enum('QA','Dev1','Dev2','Live','Local') NOT NULL,
  `hostName` varchar(50) NOT NULL,
  `tableVersion` bigint(20) unsigned NOT NULL,
  `serverName` varchar(30) NOT NULL,
  `updateTime` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  UNIQUE KEY `UX_serverList_ip_runMode` (`ip`,`runMode`),
  KEY `IX_serverList_runMode_ip` (`runMode`,`ip`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 내보낼 데이터가 선택되어 있지 않습니다.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
