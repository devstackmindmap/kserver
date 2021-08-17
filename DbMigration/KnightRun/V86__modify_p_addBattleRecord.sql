DROP PROCEDURE IF EXISTS `p_addBattleRecord`;

DELIMITER $$
CREATE PROCEDURE `p_addBattleRecord`(
	IN `$battleType` VARCHAR(50),
	IN `$userId` INT,
	IN `$enemyUserId` INT,
	IN `$battleStartTime` DATETIME,
	IN `$battleEndTime` DATETIME,
	IN `$behaviors` MEDIUMBLOB,
	IN `$battleInfo` BLOB,
	OUT `$outBehaviorsId` INT

)

BEGIN
	
	INSERT INTO battle_record_behaviors(behaviors) 
	VALUES( $behaviors);
	
	SET $outBehaviorsId = LAST_INSERT_ID();

 
	INSERT INTO battle_records( userId, battleType, behaviorsId, enemyUserId, battleStartTime, battleEndTime, isHost, battleInfo) 
	VALUES( $userId, $battleType, $outBehaviorsId,  $enemyUserId, $battleStartTime, $battleEndTime, 1, $battleInfo);

END$$
DELIMITER ;