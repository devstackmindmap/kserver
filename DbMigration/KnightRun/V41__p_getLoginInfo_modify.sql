DROP PROCEDURE `p_getLoginInfo`;

DELIMITER |
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT


)
BEGIN
	SELECT gold, gem FROM users WHERE userId = $userId;
	SELECT id, level, count, rankPoint, rankLevel FROM units WHERE userId = $userId;
	SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count, unitId FROM weapons WHERE userId = $userId;
	SELECT id, level, count, unitId FROM armors WHERE userId = $userId;
	SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
	SELECT id, boxEnergy, userEnergy, userBonusEnergy, userEnergyRecentUpdateDatetime FROM infusion_boxes WHERE userId=$userId AND type=1;
	SELECT skinId FROM skins WHERE userId=$userId;
END|
DELIMITER ;