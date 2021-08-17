CREATE TABLE `event_friends` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`friendId` INT(10) UNSIGNED NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_friends_userId_friendId` (`userId`, `friendId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
