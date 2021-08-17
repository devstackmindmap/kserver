ALTER TABLE `purchased`
	ALTER `purchaseToken` DROP DEFAULT;
ALTER TABLE `purchased`
	CHANGE COLUMN `purchaseToken` `transactionId` VARCHAR(30) NOT NULL AFTER `userId`;