CREATE TABLE `square_object` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`objectNum` INT(11) NOT NULL DEFAULT '0',
	`itemType` INT(11) NOT NULL DEFAULT '0',
	`id` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`value` INT(11) NOT NULL DEFAULT '0',
	`recentUpdateDateTime` DATETIME NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_square_object_userId_objectNum` (`userId`, `objectNum`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
