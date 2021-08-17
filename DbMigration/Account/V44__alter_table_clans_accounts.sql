ALTER TABLE `clans`
	ADD COLUMN `currentSeason` INT(11) NULL DEFAULT 0 AFTER `joinConditionRankPoint`;

ALTER TABLE `accounts`
	CHANGE COLUMN `currentSeasonRankLevel` `maxRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `currentSeason`;

ALTER TABLE `accounts`
	ADD COLUMN `currentRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `maxRankLevel`;

