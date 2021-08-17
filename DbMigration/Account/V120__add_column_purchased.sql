ALTER TABLE `purchased`
	ADD COLUMN `storeProductId` VARCHAR(50) NOT NULL AFTER `transactionId`,
	ADD COLUMN `addDateTime` DATETIME NOT NULL AFTER `storeProductId`;