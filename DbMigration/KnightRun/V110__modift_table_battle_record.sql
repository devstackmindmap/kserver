DROP TABLE  IF EXISTS `battle_records`;
DROP TABLE  IF EXISTS `battle_record_behaviors`;

CREATE TABLE `battle_records` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`battleType` VARCHAR(30) NOT NULL DEFAULT '0',
	`enemyUserId` INT(11) UNSIGNED NOT NULL,
	`battleStartTime` DATETIME NULL DEFAULT NULL,
	`battleEndTime` DATETIME NULL DEFAULT NULL,
	`isHost` INT(11) UNSIGNED NULL DEFAULT NULL,
	`battleInfo` BLOB NULL DEFAULT NULL,
	`recordKey` VARCHAR(30) NOT NULL DEFAULT '0',
	`isSaved` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	INDEX `IX_battle_records_player1UserId_battleType` (`userId`, `battleType`)
)
COMMENT='전투기록'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
