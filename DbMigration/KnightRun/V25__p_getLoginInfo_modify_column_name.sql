DROP PROCEDURE `p_getLoginInfo`;

DELIMITER $$
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT

)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION      
	BEGIN
	  ROLLBACK;
	  
	  GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
	  
	SET @text = CONCAT(@sqlstate, "|", @text);
	SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
	END;

    SELECT gold, gem, energy FROM users WHERE userId = $userId;
	SELECT id, level, count, rankPoint, rankLevel FROM units WHERE userId = $userId;
    SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count, unitId FROM weapons WHERE userId = $userId;
	SELECT id, level, count, unitId FROM armors WHERE userId = $userId;
    SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
END$$
DELIMITER ;
