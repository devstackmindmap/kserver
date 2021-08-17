CREATE TABLE `accounts` (
	`userId` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`nickName` VARCHAR(20) NOT NULL,
	`loginDateTime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`userId`),
	UNIQUE INDEX `UX_accounts_nickName` (`nickName`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
