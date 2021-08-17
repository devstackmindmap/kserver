ALTER TABLE `units`
	CHANGE COLUMN `rankLevel` `rankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `count`;

ALTER TABLE `units`
	CHANGE COLUMN `rankLevel` `currentSeasonRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `count`;

ALTER TABLE `units`
	CHANGE COLUMN `rankPoint` `currentSeasonRankPoint` INT(11) NOT NULL DEFAULT 0 AFTER `currentSeasonRankLevel`;	

ALTER TABLE `units`
	ADD COLUMN `currentSeason` INT(11) NOT NULL DEFAULT 0 AFTER `count`;

ALTER TABLE `units`
	ADD COLUMN `nextSeason` INT(11) NOT NULL DEFAULT 0 AFTER `currentSeasonRankPoint`,
	ADD COLUMN `nextSeasonRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `nextSeason`,
	ADD COLUMN `nextSeasonRankPoint` INT(11) NOT NULL DEFAULT 0 AFTER `nextSeasonRankLevel`;