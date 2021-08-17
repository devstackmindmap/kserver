CREATE TABLE `pushkeys` (
	`userId` INT(11) NOT NULL,
	`pushKeys` VARCHAR(255) NOT NULL,
	`pushAgree` BIT(1) NOT NULL,
	`pushAgreeDatetime` DATETIME NOT NULL,
	`nightPushAgree` BIT(1) NOT NULL,
	`nightPushAgreeDatetime` DATETIME NOT NULL,
	`lastLoginDate` DATETIME NOT NULL,
	PRIMARY KEY (`userId`),
	INDEX `IX_pushAgree_NightPushAgree` (`pushAgree`, `nightPushAgree`),
	INDEX `IX_pushAgree` (`pushAgree`)
)
COMMENT='유저 PUSH키 수집.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
