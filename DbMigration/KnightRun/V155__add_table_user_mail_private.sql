CREATE TABLE `user_mail_private` (
	`mailId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL DEFAULT 0,
	`isDeleted` INT(11) NOT NULL DEFAULT 0,
	`startDateTime` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
	`endDateTime` DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
	`mailTitle` VARCHAR(50) NOT NULL DEFAULT '',
	`mailText` VARCHAR(300) NOT NULL DEFAULT '',
	`isRead` INT(11) NOT NULL DEFAULT 0,
	`mailIcon` INT(11) NOT NULL DEFAULT 0,
	`productId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`mailId`),
	UNIQUE INDEX `UX_user_mail_private_userId_mailId` (`userId`, `mailId`),
	INDEX `IX_user_mail_private_userId_isDeleted_startDateTime_endDateTime` (`userId`, `isDeleted`, `endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
