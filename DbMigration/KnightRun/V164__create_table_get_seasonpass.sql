CREATE TABLE `get_seasonpass` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) NULL DEFAULT NULL,
	`seasonPassType` INT(11) NULL DEFAULT NULL,
	`season` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_get_seasonpass_userId_seasonPassType_season` (`userId`, `seasonPassType`, `season`)
)
ENGINE=InnoDB
;
