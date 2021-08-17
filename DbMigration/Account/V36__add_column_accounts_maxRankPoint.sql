ALTER TABLE `accounts`
	ADD COLUMN `maxRankPoint` INT(11) NOT NULL DEFAULT 0 AFTER `nextSeasonRankPoint`;