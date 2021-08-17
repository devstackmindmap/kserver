CREATE TABLE `products_user` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`productId` INT(11) UNSIGNED NOT NULL,
	`productTableType` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_products_user_user_id_productId` (`userId`, `productId`),
	INDEX `IX_products_user_userId_date` (`userId`, `endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
