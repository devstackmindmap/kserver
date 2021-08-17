
CREATE TABLE `quests` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`performCount` INT(11) NOT NULL DEFAULT '0',
	`receivedReward` INT(11) NOT NULL DEFAULT '0',
	`complete` INT(11) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_quests_userId_id` (`userId`, `id`),
	INDEX `IX_quest_userId_receivedReward_complete` (`userId`, `receivedReward`, `complete`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
