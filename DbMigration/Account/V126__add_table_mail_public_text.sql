CREATE TABLE `_mail_public_text` (
	`mailId` INT(10) UNSIGNED NOT NULL,
	`languageType` VARCHAR(2) NOT NULL DEFAULT '',
	`mailTitle` VARCHAR(50) NULL DEFAULT NULL,
	`mailText` VARCHAR(300) NULL DEFAULT NULL,
	PRIMARY KEY (`mailId`, `languageType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
