
ALTER TABLE `push_log`
	ADD COLUMN `reservationNum` INT NOT NULL AFTER `pushCastId`;

DROP TABLE IF EXISTS pushReservation;

CREATE TABLE `pushReservation` (
	`seq` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`Title` VARCHAR(255) NOT NULL,
	`message` VARCHAR(255) NOT NULL,
	`cond` CHAR(10) NOT NULL DEFAULT '0',
	`runMode` VARCHAR(10) NOT NULL,
	`reservationTime` DATETIME NOT NULL,
	`sendLog` BIGINT(20) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	INDEX `IX_sendLog` (`sendLog`),
	INDEX `IX_reservationTime` (`reservationTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=10
;
