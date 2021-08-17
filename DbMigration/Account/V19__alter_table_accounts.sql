ALTER TABLE `accounts`
	CHANGE COLUMN `rankLevel` `currentSeasonRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `distributeCodeSeq`;

ALTER TABLE `accounts`
	CHANGE COLUMN `rankPoint` `currentSeasonRankPoint` INT(11) NOT NULL DEFAULT 0 AFTER `currentSeasonRankLevel`;

ALTER TABLE `accounts`
	ADD COLUMN `currentSeason` INT(11) NOT NULL DEFAULT 0 AFTER `distributeCodeSeq`;

ALTER TABLE `accounts`
	ADD COLUMN `nextSeason` INT(11) NOT NULL DEFAULT 0 AFTER `currentSeasonRankPoint`,
	ADD COLUMN `nextSeasonRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `nextSeason`,
	ADD COLUMN `nextSeasonRankPoint` INT(11) NOT NULL DEFAULT 0 AFTER `nextSeasonRankLevel`;