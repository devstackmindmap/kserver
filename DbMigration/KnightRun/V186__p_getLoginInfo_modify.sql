DROP PROCEDURE  IF EXISTS `p_getLoginInfo`;

DELIMITER |
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT,
	IN `$season` INT
)
BEGIN
	SELECT gold, gem, gemPaid, level, exp, starCoin, soStartTicket, challengeCoin FROM users WHERE userId = $userId;
	SELECT eventCoin, recentUpdateDateTime FROM term_materials WHERE userId = $userId;
	SELECT id, level, count, maxRankLevel, currentRankLevel, nextSeasonRankPoint, currentSeasonRankPoint, nextSeasonRankPoint, skinId ,
			 maxVirtualRankLevel, currentVirtualRankLevel, currentVirtualRankPoint
			 FROM units WHERE userId = $userId;
	SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count FROM weapons WHERE userId = $userId;
	SELECT id, boxEnergy, userEnergy, userBonusEnergy, userEnergyRecentUpdateDatetime FROM infusion_boxes WHERE userId=$userId AND type=1;
	SELECT skinId FROM skins WHERE userId=$userId;
	SELECT id, unitId, orderNum FROM emoticons WHERE userId = $userId;
	SELECT id FROM profiles WHERE userId=$userId;
	SELECT id, performCount, receivedOrder, completedOrder, dynamicQuestId, activeTime  FROM quests WHERE userId = $userId;
	SELECT objectExp, objectLevel, coreExp, coreLevel, agencyExp, agencyLevel, userId, isActivated, activatedTime, nextInvasionTime, 
			nextInvasionLevel, nextInvasionMonsterId, powerRefreshTime, objectPower, activeObjectLevel, objectShield, planetBoxExp, 
			planetBoxLevel, invasionHistory ,coreEnergy, energyRefreshTime,
			extraCoreEnergy, extraEnergyInjectedTime, enableReward
			FROM square_object_schedule WHERE userId = $userId;
	SELECT isAlreadyFreeNicknameChange, recentDateTimeCountryChange,
			dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime, rewardedRankSeason, unlockContents,
			maxVirtualRankLevel, maxVirtualRankPoint, currentVirtualRankPoint, dailyQuestRefreshCount, dailyQuestAddcount,
			enablePassList, lastRefreshDate, addDeck
			FROM user_info WHERE userid = $userId;
	SELECT pushAgree, nightPushAgree FROM pushkeys WHERE userid = $userId;
	SELECT day, difficultLevel, clearCount, isRewarded, rewardResetCount 
			FROM challenge_stage WHERE userId = $userId AND season = $season;
END|
DELIMITER ;