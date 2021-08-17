CREATE TABLE `_challenge_events` (
	`challengeEventId` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`challengeEventNum` INT(11) UNSIGNED NULL DEFAULT 0,
	PRIMARY KEY (`challengeEventId`),
	INDEX `IX_challenge_events_startDateTime` (`endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
