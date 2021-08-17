ALTER TABLE `clans`
	ADD COLUMN `inviteCode` VARCHAR(16) NOT NULL DEFAULT '0' AFTER `memberCount`;

ALTER TABLE `clans`
	ADD COLUMN `clanExplain` VARCHAR(200) NOT NULL DEFAULT '' AFTER `inviteCode`;

ALTER TABLE `clans`
	ADD INDEX `IX_clans_inviteCode` (`inviteCode`);