DROP PROCEDURE  IF EXISTS `p_addBattleRecord`;


DELIMITER |
CREATE PROCEDURE `p_addBattleRecord`(
	IN `$battleType` VARCHAR(50),
	IN `$userId` INT,
	IN `$enemyUserId` INT,
	IN `$battleStartTime` DATETIME,
	IN `$battleEndTime` DATETIME,
	IN `$battleRecordKey` VARCHAR(50),
	IN `$battleInfo` BLOB,
	IN `$isHost` INT,
	OUT `$outPlayerSeq` INT
)

BEGIN	
	INSERT INTO battle_records( userId, battleType, enemyUserId, battleStartTime, battleEndTime, isHost, recordKey, battleInfo) 
	VALUES( $userId, $battleType, $enemyUserId, $battleStartTime, $battleEndTime, $isHost, $battleRecordKey, $battleInfo);

	SET $outPlayerSeq = LAST_INSERT_ID();
END|
DELIMITER ;