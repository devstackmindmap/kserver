
ALTER TABLE `units`
  MODIFY COLUMN `level` int(11) unsigned NOT NULL DEFAULT '1'
  , ADD COLUMN `rankLevel` int(11) unsigned NOT NULL DEFAULT '1';

DROP PROCEDURE `p_getLoginInfo`;

DELIMITER |
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT
)
BEGIN
    SELECT gold, gem, energy, bonus_energy, energy_recent_update_datetime FROM users WHERE userId = $userId;
	SELECT id, level, count, rankPoint, rankLevel FROM units WHERE userId = $userId;
    SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count, unitId FROM weapons WHERE userId = $userId;
	SELECT id, level, count, unitId FROM armors WHERE userId = $userId;
    SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
    SELECT rewardUId, rewardType, classId, rewardOpenTimeTick FROM rewards WHERE userId = $userId;
END|
DELIMITER ;

