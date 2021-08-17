ALTER TABLE `accounts`
	ADD COLUMN `limitLoginDate` DATETIME NULL DEFAULT '2019-01-01 00:00:00' AFTER `countryCode`,
	ADD COLUMN `limitLoginReason` VARCHAR(128) NULL DEFAULT '' AFTER `limitLoginDate`;