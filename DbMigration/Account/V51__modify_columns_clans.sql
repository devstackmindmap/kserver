ALTER TABLE `clans`
	CHANGE COLUMN `clanMaterUserId` `clanMasterUserId` INT(10) UNSIGNED NOT NULL AFTER `clanId`;

ALTER TABLE `clans`
	DROP COLUMN `currentSeason`;

ALTER TABLE `clans`
	DROP COLUMN `nextSeasonRankPoint`;