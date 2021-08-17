CREATE TABLE `_products_all_list` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`productId` INT(11) UNSIGNED NOT NULL,
	`storeProductId` VARCHAR(50) NOT NULL DEFAULT '',
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`productTableType` INT(11) NULL DEFAULT NULL,
	`storeType` INT(11) NULL DEFAULT NULL,
	`productBannerType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`countOfPurchases` INT(11) NULL DEFAULT NULL,
	`materialType` INT(11) NULL DEFAULT NULL,
	`saleCost` INT(11) NULL DEFAULT NULL,
	`cost` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`seq`),
	INDEX `IX_products_all_list_productId` (`productId`),
	INDEX `IX_products_all_list_startDateTime_endDateTime` (`endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `_products_event_digital` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`storeType` INT(11) NULL DEFAULT NULL,
	`productBannerType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`materialType` INT(11) NULL DEFAULT NULL,
	`saleCost` INT(11) NULL DEFAULT NULL,
	`cost` INT(11) NULL DEFAULT NULL,
	`countOfPurchases` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`productId`),
	INDEX `IX_products_event_digital_startDateTime_endDateTime` (`endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;

CREATE TABLE `_products_event_real` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`startDateTime` DATETIME NULL DEFAULT NULL,
	`endDateTime` DATETIME NULL DEFAULT NULL,
	`storeProductId` VARCHAR(50) NOT NULL DEFAULT '',
	`storeType` INT(11) NULL DEFAULT NULL,
	`productBannerType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`countOfPurchases` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`productId`),
	INDEX `IX_products_event_digital_startDateTime_endDateTime` (`endDateTime`, `startDateTime`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;

CREATE TABLE `_products_fix_digital` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`startDateTime` DATETIME NOT NULL,
	`endDateTime` DATETIME NOT NULL,
	`storeType` INT(11) NULL DEFAULT NULL,
	`productBannerType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`materialType` INT(11) NULL DEFAULT NULL,
	`saleCost` INT(11) NULL DEFAULT NULL,
	`cost` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`productId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;

CREATE TABLE `_products_fix_real` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`startDateTime` DATETIME NOT NULL,
	`endDateTime` DATETIME NOT NULL,
	`storeProductId` VARCHAR(50) NOT NULL DEFAULT '',
	`storeType` INT(11) NULL DEFAULT NULL,
	`productBannerType` INT(11) NULL DEFAULT NULL,
	`productType` INT(11) NULL DEFAULT NULL,
	`rewardId` INT(10) UNSIGNED NOT NULL DEFAULT 0,
	PRIMARY KEY (`productId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;

CREATE TABLE `_products_real_cost` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`currencyType` VARCHAR(2) NOT NULL DEFAULT '0',
	`cost` INT(11) NOT NULL DEFAULT 0,
	`saleCost` INT(11) NOT NULL DEFAULT 0,
	PRIMARY KEY (`productId`, `currencyType`)
)
COMMENT='실물 재화 비용 테이블'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `_products_text` (
	`productId` INT(11) UNSIGNED NOT NULL,
	`languageType` VARCHAR(2) NOT NULL DEFAULT '0',
	`productText` VARCHAR(20) NOT NULL DEFAULT '0',
	PRIMARY KEY (`productId`, `languageType`)
)
COMMENT='실물 재화 비용 테이블'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;


CREATE TABLE `_rewards` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`rewardId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`itemId` INT(11) UNSIGNED NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_rewards_rewardId` (`rewardId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `_items` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`itemId` INT(11) UNSIGNED NOT NULL,
	`itemType` INT(11) NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`minCount` INT(11) NOT NULL,
	`maxCount` INT(11) NOT NULL,
	`probability` INT(11) NOT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_items_itemId` (`itemId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
