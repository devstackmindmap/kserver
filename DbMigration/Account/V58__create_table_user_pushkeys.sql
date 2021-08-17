
CREATE TABLE IF NOT EXISTS `user_pushkeys` (
	`userId` INT(11) NOT NULL,
	`pushKeys` VARCHAR(255) NULL DEFAULT NULL,
	PRIMARY KEY (`userId`)
)
COMMENT='변경 혹은 accounts에 통합될수 있음.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
