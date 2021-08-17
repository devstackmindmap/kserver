DROP TABLE IF EXISTS push_log;

CREATE TABLE `push_log` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`pushCastId` VARCHAR(36) NOT NULL,
	`title` VARCHAR(255) NOT NULL,
	`message` VARCHAR(255) NOT NULL,
	`successCount` INT(11) NOT NULL DEFAULT '0',
	`failure` INT(11) NOT NULL DEFAULT '0',
	`sendDateTime` DATETIME NOT NULL,
	PRIMARY KEY (`seq`),
	INDEX `pushCastId` (`pushCastId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=5
;
