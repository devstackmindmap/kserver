DROP TABLE IF EXISTS `user_pushkeys`;

CREATE TABLE `user_pushkeys` (
	`userId` INT(11) NOT NULL,
	`pushKeys` VARCHAR(255) NOT NULL,
	`pushAgree` BIT(1) NOT NULL,
	`pushAgreeDatetime` DATETIME NOT NULL,
	`NightPushAgree` BIT(1) NOT NULL,
	`NightPushAgreeDatetime` DATETIME NOT NULL,
	PRIMARY KEY (`userId`),
	INDEX `IX_pushAgree_NightPushAgree` (`pushAgree`, `NightPushAgree`)
)
COMMENT='변경 혹은 accounts에 통합될수 있음.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;