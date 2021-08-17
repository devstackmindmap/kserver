CREATE TABLE `event_received_reward` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`rewardNum` INT(11) UNSIGNED NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_event_received_reward_userId` (`userId`, `rewardNum`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
