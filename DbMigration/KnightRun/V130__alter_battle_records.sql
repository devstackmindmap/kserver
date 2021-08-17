ALTER TABLE `battle_records`
	CHANGE COLUMN `isSaved` `isSaved` INT(11) UNSIGNED NOT NULL DEFAULT '1' AFTER `recordKey`;

DROP PROCEDURE  IF EXISTS `p_addBattleRecord`;

DELIMITER |
CREATE  PROCEDURE `p_addBattleRecord`(
	IN `$battleType` VARCHAR(50),
	IN `$userId` INT,
	IN `$enemyUserId` INT,
	IN `$battleStartTime` DATETIME,
	IN `$battleEndTime` DATETIME,
	IN `$battleRecordKey` VARCHAR(50),
	IN `$battleInfo` BLOB,
	IN `$isHost` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN	
	INSERT INTO battle_records( userId, battleType, enemyUserId, battleStartTime, battleEndTime, isHost, recordKey, battleInfo) 
	VALUES( $userId, $battleType, $enemyUserId, $battleStartTime, $battleEndTime, $isHost, $battleRecordKey, $battleInfo);

END|
DELIMITER ;