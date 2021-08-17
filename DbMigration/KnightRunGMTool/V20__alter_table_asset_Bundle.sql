ALTER TABLE `asset_bundle`
	ALTER `runMode` DROP DEFAULT;
ALTER TABLE `asset_bundle`
	CHANGE COLUMN `runMode` `runMode` VARCHAR(20) NOT NULL AFTER `name`;
	
	
DROP PROCEDURE IF EXISTS p_getFileLatestVersionOptimaizer;

DELIMITER |

CREATE  PROCEDURE `p_getFileLatestVersionOptimaizer`(
	IN `$fileType` VARCHAR(15),
	IN `$version` BIGINT,
	IN `$runMode` VARCHAR(20)



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
		SELECT originTable.Version, originTable.Name,  originTable.Bytes, originTable.FileExtensionType FROM (
			SELECT max(version) AS version FROM game_table WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN game_table AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	
	ELSEIF $fileType = 'AssetBundle' THEN
		SELECT originTable.Version, originTable.Name, originTable.Bytes, originTable.FileExtensionType  FROM (
			SELECT max(version) AS version FROM asset_bundle WHERE version > $version and runMode = $runMode GROUP BY name
		) lastestTable INNER JOIN asset_bundle AS originTable ON lastestTable.version = originTable.version
		ORDER BY originTable.orderNum DESC;
	END IF;
END|

DELIMITER ;