CREATE TABLE `pay_pending` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`transactionId` VARCHAR(50) NULL DEFAULT NULL,
	`is_pending` INT(10) NOT NULL,
	`productId` INT(10) UNSIGNED NOT NULL,
	`productTableType` INT(10) NOT NULL,
	`storeProductId` VARCHAR(50) NOT NULL DEFAULT '',
	`purchasedToken` VARCHAR(20000) NULL DEFAULT NULL,
	`platformType` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_pay_pending_user_id_transactionId` (`userId`, `transactionId`),
	INDEX `IX_pay_pending_user_id_is_pending` (`userId`, `is_pending`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
