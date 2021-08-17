CREATE TABLE `_products` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`productId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`rewardType` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(11) UNSIGNED NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_products_productId` (`productId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
