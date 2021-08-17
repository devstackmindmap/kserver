ALTER TABLE `account`
	ADD COLUMN `setRunmode` CHAR(15) NULL DEFAULT 'Dev1' AFTER `lastLoginDate`;