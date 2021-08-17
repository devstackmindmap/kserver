ALTER TABLE `units`
	ADD COLUMN `skinId` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `rankLevel`;

CREATE TABLE `skins` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`skinId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_skins_userId_skinId` (`userId`, `skinId`)
)
COMMENT='Unit skin user data table'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
