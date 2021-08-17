DELIMITER |
CREATE PROCEDURE `p_deleteAllDatas`()
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  	END;
  	
	START TRANSACTION;
	
	DELETE FROM weapons;
	DELETE FROM units;
	DELETE FROM cards;
	DELETE FROM infusion_boxes;
	DELETE FROM stage_levels;
	DELETE FROM decks;
	DELETE FROM corrections;
	DELETE FROM armors;
	DELETE FROM users;
	DELETE FROM accounts;
	
	COMMIT;
END|
DELIMITER ;

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
  	DELETE FROM accounts WHERE userId = $userId;
END|
DELIMITER ;