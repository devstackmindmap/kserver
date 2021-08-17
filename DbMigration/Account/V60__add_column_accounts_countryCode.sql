ALTER TABLE `accounts`
	ADD COLUMN `countryCode` VARCHAR(2) NOT NULL DEFAULT '' AFTER `clanName`;