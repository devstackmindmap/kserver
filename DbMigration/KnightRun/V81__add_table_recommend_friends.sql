CREATE TABLE `recommend_friends` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`friendId` INT(10) UNSIGNED NOT NULL,
	`isSigned` INT(10) NOT NULL,
	`recommendDateTime` DATETIME NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_recommend_friends_userId` (`userId`, `friendId`),
	INDEX `IX_recommend_friends_userId_isSigned_recommendDateTime` (`userId`, `isSigned`, `recommendDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
