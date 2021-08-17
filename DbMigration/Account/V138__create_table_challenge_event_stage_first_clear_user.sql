CREATE TABLE `challenge_event_stage_first_clear_user` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`challengeEventId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`difficultLevel` INT(11) NULL DEFAULT NULL,
	`insertDateTime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_challenge_eventId_difficultLevel` (`challengeEventId`, `difficultLevel`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
