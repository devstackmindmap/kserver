-- Script generated by MySQL Compare 1.0.1.4 on 2018-10-31 오전 11:03:20

CREATE TABLE `knightrun_gmtool`.`asset_bundle` (
  `version` bigint(20) unsigned NOT NULL auto_increment,
  `name` varchar(50) NOT NULL,
  `runMode` varchar(10) NOT NULL,
  `url` varchar(150) NOT NULL,
  `comment` text ,
  `createDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`version`),
  KEY `IX_asset_bundle_name_runMode`(`name`, `runMode`)
) ENGINE=InnoDB;

DROP INDEX `IX_gameTable_tableType_serverType` ON `knightrun_gmtool`.`game_table`;

ALTER TABLE `knightrun_gmtool`.`game_table`
  CHANGE COLUMN `tableType` `name` varchar(50) NOT NULL;

CREATE INDEX `IX_gameTable_name_runMode` ON `knightrun_gmtool`.`game_table`(`name`, `runMode`);

ALTER TABLE `knightrun_gmtool`.`server_list`
  MODIFY COLUMN `tableVersion` bigint(20) unsigned NOT NULL DEFAULT 0
  , ADD COLUMN `assetBundleVersion` bigint(20) unsigned NOT NULL DEFAULT 0
  , CHANGE COLUMN `updateTime` `tableUpdateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
  , ADD COLUMN `assetBundleUpdateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP;

DELIMITER |
CREATE PROCEDURE `p_getFileBetweenVersion`(
	IN `$fileType` VARCHAR(15),
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

	IF $fileType = 'Table' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM game_table WHERE version > $fromVersion and version <= $toVersion and runMode = $runMode GROUP BY name ORDER BY version ASC;	
		
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM asset_bundle WHERE version > $fromVersion and version <= $toVersion and runMode = $runMode GROUP BY name ORDER BY version ASC;	
	END IF;
END|
DELIMITER ;

DELIMITER |
CREATE PROCEDURE `p_getFileLatestVersion`(
	IN `$fileType` VARCHAR(15),
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

	IF $fileType = 'Table' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM game_table WHERE version > $version and runMode = $runMode GROUP BY name ORDER BY version ASC;
		
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM asset_bundle WHERE version > $version and runMode = $runMode GROUP BY name ORDER BY version ASC;
	END IF;
END|
DELIMITER ;

DELIMITER |
CREATE PROCEDURE `p_getFileWithinVersion`(
	IN `$fileType` VARCHAR(15),
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

	IF $fileType = 'Table' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM game_table WHERE version <= $version and runMode = $runMode GROUP BY name ORDER BY version ASC;
		
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT max(version) as version, max(url) as url, name
		FROM asset_bundle WHERE version <= $version and runMode = $runMode GROUP BY name ORDER BY version ASC;
	END IF;
END|
DELIMITER ;

DELIMITER |
CREATE PROCEDURE `p_updateFileVersion`(
	IN `$fileType` VARCHAR(15),
	IN `$hostName` VARCHAR(50),
	IN `$ip` VARCHAR(30),
	IN `$version` BIGINT,
	IN `$runMode` VARCHAR(10),
	IN `$serverName` VARCHAR(30),
	IN `$isRunning` BIT,
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
  		IF $fileType = 'Table' THEN
	  		INSERT INTO server_list(ip, hostName, tableVersion, runMode, serverName, isRunning, tableUpdateTime) VALUES($ip, $hostName, $version, $runMode, $serverName, $isRunning, $updateTime)
	  		ON DUPLICATE KEY UPDATE tableVersion = $version, isRunning = $isRunning, tableUpdateTime = $updateTime;
	  		
	   ELSEIF $fileType = 'AssetBundle' THEN
	   	INSERT INTO server_list(ip, hostName, assetBundleVersion, runMode, serverName, isRunning, assetBundleUpdateTime) VALUES($ip, $hostName, $version, $runMode, $serverName, $isRunning, $updateTime)
	  		ON DUPLICATE KEY UPDATE assetBundleVersion = $version, isRunning = $isRunning, assetBundleUpdateTime = $updateTime;
	   END IF;
  	COMMIT;
END|
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_getGameTableBetweenVersion`;

DROP PROCEDURE `knightrun_gmtool`.`p_getGameTableLatestVersion`;

DROP PROCEDURE `knightrun_gmtool`.`p_getGameTableWithinVersion`;

DROP PROCEDURE `knightrun_gmtool`.`p_updateGameTableToServer`;
