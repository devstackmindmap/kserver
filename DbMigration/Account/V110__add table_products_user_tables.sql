CREATE TABLE `_products_user_digital` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`saleDurationHour` INT(11) NOT NULL,
	`storeType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`materialType` INT(11) NULL DEFAULT NULL,
	`saleCost` INT(11) NULL DEFAULT NULL,
	`cost` INT(11) NULL DEFAULT NULL,
	`countOfPurchases` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`productId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;


CREATE TABLE `_products_user_real` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`platform` INT(11) NOT NULL,
	`storeProductId` VARCHAR(50) NOT NULL DEFAULT '',
	`saleDurationHour` INT(11) NOT NULL,
	`storeType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`saleCost` INT(11) NULL DEFAULT NULL,
	`cost` INT(11) NULL DEFAULT NULL,
	`countOfPurchases` INT(11) NULL DEFAULT NULL,
	PRIMARY KEY (`productId`, `platform`),
	INDEX `IX_products_user_real_storeProductId` (`storeProductId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
