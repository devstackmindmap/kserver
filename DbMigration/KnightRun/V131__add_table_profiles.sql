CREATE TABLE `profiles` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	`Id` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_profiles_userId_id` (`userId`, `Id`)
)
COMMENT='Unit skin user data table'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
