CREATE TABLE `battle_record_behaviors` (
	`behaviorsId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`behaviors` MEDIUMBLOB NULL DEFAULT NULL,
	PRIMARY KEY (`behaviorsId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;