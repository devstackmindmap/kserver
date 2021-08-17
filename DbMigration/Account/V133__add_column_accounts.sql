ALTER TABLE `accounts`
	ADD COLUMN `isActivatedSO` INT NOT NULL DEFAULT '0' AFTER `limitLoginReason`;
