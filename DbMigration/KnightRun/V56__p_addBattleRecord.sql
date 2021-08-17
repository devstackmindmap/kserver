DROP PROCEDURE  IF EXISTS `p_addBattleRecord`;

DELIMITER |
CREATE PROCEDURE `p_addBattleRecord`(
	IN `$version` INT,
	IN `$battleType` VARCHAR(50),
	IN `$player1userid` INT,
	IN `$player2userid` INT,
	IN `$behaviors` MEDIUMBLOB

)
COMMENT '전투기록 추가'
BEGIN
	INSERT INTO battle_records(VERSION, BATTLETYPE, PLAYER1_USERID, PLAYER2_USERID, BEHAVIORS) 
	VALUES($version, $battleType, $player1userid, $player2userid, $behaviors);
END|
DELIMITER ;