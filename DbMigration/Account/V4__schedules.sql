CREATE TABLE `_schedules` (
	`scheduleId` INT(11) NOT NULL AUTO_INCREMENT,
	`scheduleType` INT(11) NULL DEFAULT NULL,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`scheduleId`),
	INDEX `IX_schedules_endDateTime_startDateTime` (`startDateTime`, `endDateTime`)
)
COMMENT='통합 스케줄 테이블'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
