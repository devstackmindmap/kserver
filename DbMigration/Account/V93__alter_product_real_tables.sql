ALTER TABLE `_products_event_real`
	ADD COLUMN `saleCost` INT(11) NULL DEFAULT NULL AFTER `productType`,
	ADD COLUMN `cost` INT(11) NULL DEFAULT NULL AFTER `saleCost`;

ALTER TABLE `_products_fix_real`
	ADD COLUMN `saleCost` INT(11) NULL DEFAULT NULL AFTER `productType`,
	ADD COLUMN `cost` INT(11) NULL DEFAULT NULL AFTER `saleCost`;