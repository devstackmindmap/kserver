ALTER TABLE `square_object_schedule`
	CHANGE COLUMN `activatedTime` `activatedTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP AFTER `isActivated`,
	CHANGE COLUMN `nextInvasionTime` `nextInvasionTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP AFTER `activatedTime`,
	ADD COLUMN `planetBoxLevel` INT(11) UNSIGNED NOT NULL DEFAULT 1 AFTER `planetBoxExp`,
	ADD INDEX `IX_square_objects_schedule_isActived` (`isActivated`),
	DROP COLUMN `열 13`;