ALTER TABLE `_common`
	CHANGE COLUMN `currentSeason` `commonValue` INT(11) NULL DEFAULT NULL AFTER `commonId`,
	CHANGE COLUMN `currentSeasonStartDateTime` `commonStartDateTime` DATETIME NULL DEFAULT NULL AFTER `commonValue`,
	CHANGE COLUMN `nextSeasonStartDateTime` `commonNextStartDateTime` DATETIME NULL DEFAULT NULL AFTER `commonStartDateTime`;

ALTER TABLE `_common`
	ADD COLUMN `commonValue2` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `commonValue`;