CREATE TABLE `corrections` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`type` TINYINT(4) NOT NULL,
	`id` INT(10) UNSIGNED NOT NULL,
	`correction` INT(10) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_corrections_userId_type_id` (`id`, `userId`, `type`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
