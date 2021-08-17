ALTER TABLE `square_object_schedule`
	ADD COLUMN `enableContents` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `objectLevel`;
