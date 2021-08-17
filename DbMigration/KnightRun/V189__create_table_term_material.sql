CREATE TABLE `term_materials` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`eventCoin` INT(11) NOT NULL DEFAULT 0,
	`recentUpdateDateTime` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_users_userId` (`userId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
