
ALTER TABLE `game_table`
	ADD COLUMN `Bytes` INT NOT NULL DEFAULT '0' AFTER `isSelected`;
	
ALTER TABLE `asset_bundle`
	ADD COLUMN `Bytes` INT NOT NULL DEFAULT '0' AFTER `mdChecksum`;	
	

DROP PROCEDURE IF EXISTS `p_getFileLatestVersion`;


DELIMITER $$
CREATE  PROCEDURE `p_getFileLatestVersion`(
	IN `$fileType` VARCHAR(15),
	IN `$version` BIGINT,
	IN `$runMode` VARCHAR(10)


)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
COMMENT ''
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	IF $fileType = 'Table' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode, originTable.mdChecksum, originTable.Bytes FROM (
			SELECT max(version) AS version FROM game_table WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN game_table AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT originTable.version, originTable.name, originTable.url, originTable.runMode, originTable.mdChecksum, originTable.Bytes  FROM (
			SELECT max(version) AS version FROM asset_bundle WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN asset_bundle AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	END IF;
END$$
DELIMITER ;
