ALTER TABLE `units`
	CHANGE COLUMN `currentSeasonRankLevel` `maxRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `count`;

ALTER TABLE `units`
	ADD COLUMN `currentRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `maxRankLevel`;