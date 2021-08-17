CREATE TABLE `_country_codes` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`country_code` CHAR(2) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

INSERT INTO `_country_codes` (`seq`, `country_code`) VALUES (1, 'KR');
