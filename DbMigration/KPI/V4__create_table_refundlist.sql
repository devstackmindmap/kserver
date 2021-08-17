CREATE TABLE `refundlist` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`transactionId` VARCHAR(50) NOT NULL,
	`productId` VARCHAR(100) NULL DEFAULT NULL,
	`storeProductId` VARCHAR(50) NULL DEFAULT NULL,
	`Refunding` TINYINT(4) NOT NULL,
	`orderdate` DATE NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `transactionId` (`transactionId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
