
ALTER TABLE `accounts`
	ADD COLUMN `initDataVersion` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `friendCode`;