ALTER TABLE `pay_pending`
	ADD COLUMN `payedTime` DATETIME NOT NULL AFTER `platformType`;
