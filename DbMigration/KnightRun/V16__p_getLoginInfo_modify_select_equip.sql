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

    SELECT gold, gem FROM users WHERE userId = $userId;
	SELECT id, level, count FROM units WHERE userId = $userId;
    SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count, unitId FROM weapons WHERE userId = $userId;
	SELECT id, level, count, unitId FROM armors WHERE userId = $userId;
    SELECT rankType, rankPoint, rankTierMatchingId FROM ranks WHERE userId = $userId;
    SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
    SELECT rewardUId, rewardType, classId, rewardOpenTimeTick FROM rewards WHERE userId = $userId;
END$$
DELIMITER ;
