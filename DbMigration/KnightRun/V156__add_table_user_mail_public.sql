CREATE TABLE `user_mail_public` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`mailId` INT(10) UNSIGNED NOT NULL,
	`isDeleted` INT(11) NULL DEFAULT NULL,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`isRead` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_user_mail_public_userId_mailId` (`userId`, `mailId`),
	INDEX `IX_user_mail_public_userId_isDeleted_endDateTime_startDateTime` (`userId`, `isDeleted`, `endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
