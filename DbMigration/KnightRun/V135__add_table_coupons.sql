CREATE TABLE `coupons` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`couponCode` VARCHAR(16) NULL DEFAULT NULL,
	`getDateTime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_coupons_userId_couponCode` (`userId`, `couponCode`),
	INDEX `IX_coupons_couponCode` (`couponCode`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
