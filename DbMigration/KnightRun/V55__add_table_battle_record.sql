CREATE TABLE `battle_records` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`version` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`battleType` VARCHAR(50) NOT NULL DEFAULT '0',
	`player1UserId` INT(11) UNSIGNED NOT NULL,
	`player2UserId` INT(11) UNSIGNED NOT NULL,
	`behaviors` MEDIUMBLOB NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_battle_records_player1UserId_battleType` (`player1UserId`, `battleType`)
)
COMMENT='전투기록'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=4
;
