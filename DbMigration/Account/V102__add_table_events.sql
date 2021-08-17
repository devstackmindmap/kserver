CREATE TABLE `_events` (
	`eventId` INT(11) NOT NULL AUTO_INCREMENT,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`eventType` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`eventId`),
	INDEX `IX_events_startDateTime` (`endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
