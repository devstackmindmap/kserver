
DROP INDEX `IX_asset_bundle_name_runMode` ON `knightrun_gmtool`.`asset_bundle`;

DROP INDEX `IX_gameTable_name_runMode` ON `knightrun_gmtool`.`game_table`;

ALTER TABLE `knightrun_gmtool`.`account`
  ADD COLUMN `lastLoginDate` timestamp NOT NULL DEFAULT current_timestamp();

ALTER TABLE `knightrun_gmtool`.`asset_bundle`
  ADD COLUMN `orderNum` int(11) NOT NULL DEFAULT '0';

CREATE INDEX `IX_asset_bundle_name_runMode_orderNum` ON `knightrun_gmtool`.`asset_bundle`(`orderNum`, `name`, `runMode`);

ALTER TABLE `knightrun_gmtool`.`game_table`
  ADD COLUMN `orderNum` int(11) NOT NULL DEFAULT '0'
  , ADD COLUMN `isSelected` bit(1) NOT NULL DEFAULT b'1';

CREATE INDEX `IX_gameTable_name_runMode_orderNum` ON `knightrun_gmtool`.`game_table`(`name`, `runMode`, `orderNum`);

DROP PROCEDURE `knightrun_gmtool`.`p_getFileBetweenVersion`;

DELIMITER $$
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
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;


	IF $fileType = 'Table' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode, originTable.isSelected FROM (
			SELECT max(version) AS version FROM game_table WHERE version > $fromVersion and version <= $toVersion and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN game_table AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode FROM (
			SELECT max(version) AS version FROM asset_bundle WHERE version > $fromVersion and version <= $toVersion and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN asset_bundle AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	END IF;
END$$
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_getFileLatestVersion`;

DELIMITER $$
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
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	IF $fileType = 'Table' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode FROM (
			SELECT max(version) AS version FROM game_table WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN game_table AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode FROM (
			SELECT max(version) AS version FROM asset_bundle WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN asset_bundle AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	END IF;
END$$
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_getFileWithinVersion`;

DELIMITER $$
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
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	IF $fileType = 'Table' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode FROM (
			SELECT max(version) AS version FROM game_table WHERE version <= $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN game_table AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
		
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode FROM (
			SELECT max(version) AS version FROM asset_bundle WHERE version <= $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN asset_bundle AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	END IF;
END$$
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_transferLatestFile`;

DELIMITER $$
CREATE PROCEDURE `p_transferLatestFile`(
	IN `$fileType` VARCHAR(15),
	IN `$from_runMode` VARCHAR(10),
	IN `$to_runMode` VARCHAR(10),
	IN `$comment` TINYTEXT

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
		IF $fileType = 'Table' THEN
			SELECT @count := COUNT(*) FROM game_table WHERE runMode = $to_runMode;
			IF @count = 0 THEN
				INSERT INTO game_table(name, url, runMode, comment, createDate)
				SELECT name, max(url) as url, $to_runMode as runMode, $comment as comment, CURRENT_TIMESTAMP() as createDate
				FROM game_table WHERE version > 0 and runMode = $from_runMode GROUP BY name;
				
			ELSE 
				INSERT INTO game_table(tableType, url, runMode, comment, createDate) 
				SELECT from_runMode.name as name, from_runMode.url as url, $to_runMode as runMode, $comment as comment, CURRENT_TIME() as createDate FROM (
					SELECT name, max(version) as version, max(url) as url, runMode FROM game_table WHERE runMode = $from_runMode GROUP BY name
				) from_runMode INNER JOIN (
					SELECT name, max(version) as version, max(url) as url, runMode FROM game_table WHERE runMode = $to_runMode GROUP BY name
				) to_runMode 
				ON from_runMode.name = to_runMode.name and from_runMode.version > to_runMode.version;
			END IF;
			
		ELSEIF $fileType = 'AssetBundle' THEN
			SELECT @count := COUNT(*) FROM asset_bundle WHERE runMode = $to_runMode;
			IF @count = 0 THEN
				INSERT INTO asset_bundle(name, url, runMode, comment, createDate)
				SELECT name, max(url) as url, $to_runMode as runMode, $comment as comment, CURRENT_TIMESTAMP() as createDate
				FROM asset_bundle WHERE version > 0 and runMode = $from_runMode GROUP BY name;
				
			ELSE 
				INSERT INTO asset_bundle(tableType, url, runMode, comment, createDate) 
				SELECT from_runMode.name as name, from_runMode.url as url, $to_runMode as runMode, $comment as comment, CURRENT_TIME() as createDate FROM (
					SELECT name, max(version) as version, max(url) as url, runMode FROM asset_bundle WHERE runMode = $from_runMode GROUP BY name
				) from_runMode INNER JOIN (
					SELECT name, max(version) as version, max(url) as url, runMode FROM asset_bundle WHERE runMode = $to_runMode GROUP BY name
				) to_runMode 
				ON from_runMode.name = to_runMode.name and from_runMode.version > to_runMode.version;
			END IF;
		END IF;
	COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_updateFileVersion`;

DELIMITER $$
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
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
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
END$$
DELIMITER ;

DROP PROCEDURE `knightrun_gmtool`.`p_transferLatestGameTable`;
