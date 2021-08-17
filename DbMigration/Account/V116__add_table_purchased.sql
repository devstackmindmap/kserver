CREATE TABLE `purchased` (
	`seq` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) NOT NULL,
	`transactionId` VARCHAR(30) NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_purchased_transactionId` (`transactionId`),
	INDEX `IX_purchased_userId` (`userId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
