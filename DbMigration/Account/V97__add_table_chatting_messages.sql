CREATE TABLE `chatting_messages` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`chattingKey` VARCHAR(20) NULL DEFAULT NULL,
	`chattingDateTime` DATETIME NULL DEFAULT NULL,
	`chattingType` INT(11) NULL DEFAULT NULL,
	`chattingMessage` VARCHAR(168) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_chatting_messages_chattingKey_chattingDateTime` (`chattingKey`, `chattingDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
