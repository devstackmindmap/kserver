DROP PROCEDURE  IF EXISTS `p_deleteUserDatas`;

DELIMITER |
CREATE PROCEDURE `p_deleteUserDatas`(
	IN `$userId` INT
)
BEGIN
  	DELETE FROM armors WHERE userId = $userId;
  	DELETE FROM cards WHERE userId = $userId;
  	DELETE FROM corrections WHERE userId = $userId;
  	DELETE FROM decks WHERE userId = $userId;
  	DELETE FROM infusion_boxes WHERE userId = $userId;
  	DELETE FROM stage_levels WHERE userId = $userId;
	DELETE FROM units WHERE userId = $userId;
	DELETE FROM weapons WHERE userId = $userId;
  	DELETE FROM users WHERE userId = $userId;
  	DELETE FROM friends WHERE userId = $userId;
  	DELETE FROM ranks WHERE userId = $userId;
  	DELETE FROM skins WHERE userId = $userId;
	DELETE FROM square_object WHERE userId = $userId;
	DELETE FROM battle_records WHERE userId = $userId;
	DELETE FROM battle_record_behaviors WHERE userId = $userId;
	DELETE FROM inprogress_stage WHERE userId = $userId;
END|
DELIMITER ;