CREATE TABLE `distribute_code` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`distributeCode` VARCHAR(6) NOT NULL,
	`company` VARCHAR(50) NOT NULL,
	`name` VARCHAR(50) NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_distribute_code_distributeCode` (`distributeCode`)
)
ENGINE=InnoDB
;
