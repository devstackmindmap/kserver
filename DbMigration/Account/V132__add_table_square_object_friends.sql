
CREATE TABLE `square_object_friends` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`receivedUserId` INT(10) UNSIGNED NOT NULL,
	`sendUserId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
	`receivedDate` DATETIME NOT NULL DEFAULT '2019-01-01 00:00:00',
	PRIMARY KEY (`seq`),
	INDEX `IX_receivedUserId_receivedDate` (`receivedDate`, `receivedUserId`),
	INDEX `IX_sendUserId_receivedDate` (`sendUserId`, `receivedDate`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
