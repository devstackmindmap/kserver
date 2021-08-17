CREATE TABLE `clan_members` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`clanId` INT(10) UNSIGNED NOT NULL DEFAULT 0,
	`userId` INT(10) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_clan_members_userId` (`userId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;