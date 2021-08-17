ALTER TABLE `_products_all_list`
	CHANGE COLUMN `storeProductId` `aosStoreProductId` VARCHAR(50) NOT NULL DEFAULT '' AFTER `productId`,
	ADD COLUMN `iosStoreProductId` VARCHAR(50) NOT NULL DEFAULT '' AFTER `aosStoreProductId`;

ALTER TABLE `_products_all_list`
	DROP COLUMN `platform`,
	DROP INDEX `IX_products_all_list_startDateTime_endDateTime`,
	ADD INDEX `IX_products_all_list_startDateTime_endDateTime` (`endDateTime`, `startDateTime`);