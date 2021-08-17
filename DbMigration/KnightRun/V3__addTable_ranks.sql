CREATE TABLE `ranks` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`rankType` INT(10) UNSIGNED NOT NULL,
	`rankPoint` INT(10) UNSIGNED NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_ranks_userId_rankType` (`userId`, `rankType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
