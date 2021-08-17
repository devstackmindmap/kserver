CREATE TABLE `infusion_boxes` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`type` TINYINT(3) NOT NULL,
	`id` INT(10) UNSIGNED NOT NULL,
	`boxEnergy` INT(10) NOT NULL DEFAULT '0',
	`userEnergy` INT(10) NOT NULL DEFAULT '0',
	`userBonusEnergy` INT(10) NOT NULL DEFAULT '0',
	`userEnergyRecentUpdateDatetime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_boxes_userId_type` (`userId`, `type`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
