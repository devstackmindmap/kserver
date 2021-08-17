ALTER TABLE `clans`
	ADD COLUMN `countryCode` VARCHAR(2) NOT NULL DEFAULT '' AFTER `clanId`;

ALTER TABLE `clans`
	CHANGE COLUMN `clanPublicType` `clanPublicType` INT(11) NULL DEFAULT NULL AFTER `countryCode`;

ALTER TABLE `clans`
	CHANGE COLUMN `joinConditionRankPoint` `joinConditionRankPoint` INT(11) NULL DEFAULT 0 AFTER `clanPublicType`;

ALTER TABLE `clans`
	ADD INDEX `IX_clans_countryCode_clanPublicType_joinConditionRankPoint` (`countryCode`, `clanPublicType`, `joinConditionRankPoint`, `clanId`);