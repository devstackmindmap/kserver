CREATE TABLE `purchased` (
	`seq` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) NOT NULL,
	`purchaseToken` VARCHAR(1000) NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_purchased_userId_purchaseToken` (`userId`, `purchaseToken`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
