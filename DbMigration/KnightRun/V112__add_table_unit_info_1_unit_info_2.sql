CREATE TABLE `user_info_1` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userid` INT(10) UNSIGNED NULL DEFAULT NULL,
	`userInfoType` INT(10) NULL DEFAULT NULL,
	`userValue` TINYINT(4) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_user_info_1_userId_userInfoType` (`userid`, `userInfoType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;


CREATE TABLE `user_info_2` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userid` INT(10) UNSIGNED NULL DEFAULT NULL,
	`userInfoType` INT(10) NULL DEFAULT NULL,
	`userValue` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_user_info_2_userId_userInfoType` (`userid`, `userInfoType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
