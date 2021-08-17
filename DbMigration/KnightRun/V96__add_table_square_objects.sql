CREATE TABLE `square_object_schedule` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`isActivated` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`activatedTime` DATETIME NOT NULL,
	`nextInvasionTime` DATETIME NOT NULL,
	`nextInvasionLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`powerRefreshTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`objectPower` INT(11) NOT NULL DEFAULT '0',
	`objectLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`objectShield` INT(11) NOT NULL DEFAULT '0',
	`planetBoxExp` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`invasionHistory` BLOB NULL DEFAULT NULL,
	`열 13` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_square_objects_schedule_userId` (`userId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
