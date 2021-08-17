CREATE TABLE `_coupon` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`couponCode` VARCHAR(16) NULL DEFAULT NULL,
	`couponType` INT(11) NULL DEFAULT NULL,
	`validDateTime` DATETIME NULL DEFAULT NULL,
	`productId` INT(10) UNSIGNED NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_coupon_couponCode` (`couponCode`)
)
COMMENT='코드 보상'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
