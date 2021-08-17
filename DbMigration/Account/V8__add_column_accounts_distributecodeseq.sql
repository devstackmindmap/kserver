ALTER TABLE `accounts`
	ADD COLUMN `distributeCodeSeq` INT NULL DEFAULT NULL AFTER `joinDateTime`;