ALTER TABLE `accounts`
	ADD COLUMN `wins` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `isActivatedSO`;