CREATE TABLE `clans` (
	`clanId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`clanMaterUserId` INT(10) UNSIGNED NOT NULL,
	`clanName` VARCHAR(50) NOT NULL,
	`clanSymbolId` INT(11) NULL DEFAULT NULL,
	`clanPublicType` INT(11) NULL DEFAULT NULL,
	`joinConditionRankPoint` INT(11) NULL DEFAULT 0,
	`currentSeasonRankPoint` INT(11) NULL DEFAULT 0,
	`nextSeasonRankPoint` INT(11) NULL DEFAULT 0,
	PRIMARY KEY (`clanId`),
	UNIQUE INDEX `UX_clans_clanName` (`clanName`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
