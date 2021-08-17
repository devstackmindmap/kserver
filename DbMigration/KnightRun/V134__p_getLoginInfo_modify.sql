DROP PROCEDURE  IF EXISTS `p_getLoginInfo`;

DELIMITER |
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT


)
BEGIN
	SELECT gold, gem, gemPaid, level, exp FROM users WHERE userId = $userId;
	SELECT id, level, count, maxRankLevel, currentRankLevel, currentSeasonRankPoint, nextSeasonRankPoint, skinId FROM units WHERE userId = $userId;
	SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count FROM weapons WHERE userId = $userId;
	SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
	SELECT id, boxEnergy, userEnergy, userBonusEnergy, userEnergyRecentUpdateDatetime FROM infusion_boxes WHERE userId=$userId AND type=1;
	SELECT skinId FROM skins WHERE userId=$userId;
	SELECT id, unitId, orderNum FROM emoticons WHERE userId = $userId;
	SELECT id FROM profiles WHERE userId=$userId;
	SELECT id, performCount, receivedOrder, completedOrder FROM quests WHERE userId = $userId;
	SELECT objectExp, objectLevel, coreExp, coreLevel, agencyExp, agencyLevel, userId, isActivated, activatedTime, nextInvasionTime, 
			nextInvasionLevel, nextInvasionMonsterId, powerRefreshTime, objectPower, activeObjectLevel, objectShield, planetBoxExp, 
			planetBoxLevel, invasionHistory ,coreEnergy, energyRefreshTime
			FROM square_object_schedule WHERE userId = $userId;
	SELECT isAlreadyFreeNicknameChange, recentDateTimeCountryChange,
			dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime, rewardedRankSeason, unlockContents
			FROM user_info WHERE userid = $userId;
END|
DELIMITER ;