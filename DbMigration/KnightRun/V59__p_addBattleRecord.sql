DROP PROCEDURE  IF EXISTS `p_addBattleRecord`;

DELIMITER |
CREATE PROCEDURE `p_addBattleRecord`(
	IN `$version` INT,
	IN `$battleType` VARCHAR(50),
	IN `$userId` INT,
	IN `$enemyUserId` INT,
	IN `$nickName` VARCHAR(20),
	IN `$enemyNickName` VARCHAR(20),
	IN `$winner` INT,
	IN `$battleStartTime` DATETIME,
	IN `$battleEndTime` DATETIME,
	IN `$behaviors` MEDIUMBLOB

)
COMMENT '전투기록 추가'
BEGIN

	INSERT INTO battle_record_behaviors(behaviors) 
	VALUES( $behaviors);
	
	SET @behaviorsId = LAST_INSERT_ID();


	INSERT INTO battle_records(version, userId, battleType, behaviorsId, enemyUserId, nickName, enemyNickName, winner, battleStartTime, battleEndTime) 
	VALUES($version, $userId, $battleType, @behaviorsId,  $enemyUserId, $nickName, $enemyNickName, $winner, $battleStartTime, $battleEndTime);

	IF $enemyUserId > 0 THEN
		INSERT INTO battle_records(version, userId, battleType, behaviorsId, enemyUserId, nickName, enemyNickName, winner, battleStartTime, battleEndTime) 
		VALUES($version, $enemyUserId, $battleType, @behaviorsId,  $userId, $enemyNickName, $nickName, $winner, $battleStartTime, $battleEndTime);
	END IF;

END|
DELIMITER ;