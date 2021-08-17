DROP TABLE square_objects;
DROP TABLE square_object_schedule;

CREATE TABLE `square_object_schedule` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`isActivated` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`activatedTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`nextInvasionTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`nextInvasionLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`powerRefreshTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`objectPower` INT(11) NOT NULL DEFAULT '0',
	`activeObjectLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`objectShield` INT(11) NOT NULL DEFAULT '0',
	`planetBoxExp` INT(11) NOT NULL DEFAULT '0',
	`planetBoxLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`coreEnergy` INT(11) NOT NULL DEFAULT '0',
	`energyRefreshTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`invasionHistory` BLOB NULL DEFAULT NULL,
	`objectExp` INT(11) NOT NULL DEFAULT '0',
	`objectLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`coreExp` INT(11) NOT NULL DEFAULT '0',
	`coreLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`agencyExp` INT(11) NOT NULL DEFAULT '0',
	`agencyLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1',
	`hasReward` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_square_objects_schedule_userId` (`userId`),
	INDEX `IX_square_objects_schedule_isActived` (`isActivated`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

