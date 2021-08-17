DROP TABLE ranks;

CREATE TABLE `ranks` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
	`rankType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
	`rankLevel` INT(10) UNSIGNED NOT NULL DEFAULT '0',
	`rankPoint` INT(10) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_ranks_userId_rankType` (`userId`, `rankType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
