
CREATE TABLE `emoticons` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`orderNum` INT(11) NOT NULL DEFAULT '-1',
	`unitId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_emoticons_userId_id` (`userId`, `id`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
