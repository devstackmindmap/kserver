DROP PROCEDURE IF EXISTS `p_addBattleRecord`;

DELIMITER $$
CREATE PROCEDURE `p_addBattleRecord`(
	IN `$battleType` VARCHAR(50),
	IN `$userId` INT,
	IN `$enemyUserId` INT,
	IN `$battleStartTime` DATETIME,
	IN `$battleEndTime` DATETIME,
	IN `$behaviors` MEDIUMBLOB,
	IN `$battleInfo` BLOB

)

BEGIN

	INSERT INTO battle_record_behaviors(behaviors, battleInfo) 
	VALUES( $behaviors, $battleInfo);
	
	SET @behaviorsId = LAST_INSERT_ID();

 
	INSERT INTO battle_records( userId, battleType, behaviorsId, enemyUserId, battleStartTime, battleEndTime) 
	VALUES( $userId, $battleType, @behaviorsId,  $enemyUserId, $battleStartTime, $battleEndTime);

	IF $enemyUserId > 0 THEN
		INSERT INTO battle_records( userId, battleType, behaviorsId, enemyUserId, battleStartTime, battleEndTime) 
		VALUES( $enemyUserId, $battleType, @behaviorsId,  $userId, $battleStartTime, $battleEndTime);
	END IF;
END$$
DELIMITER ;