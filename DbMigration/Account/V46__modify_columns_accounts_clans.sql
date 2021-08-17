ALTER TABLE `clans`
	ADD COLUMN `memberCount` INT(11) NULL DEFAULT 0 AFTER `nextSeasonRankPoint`;


ALTER TABLE `clans`
	CHANGE COLUMN `clanSymbolId` `clanSymbolId` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `clanName`;


ALTER TABLE `accounts`
	ADD COLUMN `clanName` VARCHAR(50) NOT NULL DEFAULT '' AFTER `email`;

ALTER TABLE `accounts`
	CHANGE COLUMN `profileIconId` `profileIconId` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `rankVictoryCount`;