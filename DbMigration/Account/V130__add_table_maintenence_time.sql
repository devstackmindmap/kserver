
CREATE TABLE `_maintenance_time` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`startTime` DATETIME NOT NULL,
	`endTime` DATETIME NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_endTime` (`endTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
