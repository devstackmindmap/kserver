ALTER TABLE `clans`
	CHANGE COLUMN `currentSeasonRankPoint` `rankPoint` INT(11) NULL DEFAULT 0 AFTER `joinConditionRankPoint`;