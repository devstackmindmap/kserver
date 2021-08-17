ALTER TABLE `_common`
	CHANGE COLUMN `commonValue` `commonValue` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `commonId`,
	CHANGE COLUMN `commonValue3` `commonValue3` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `commonValue2`,
	CHANGE COLUMN `commonValue4` `commonValue4` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `commonValue3`,
	CHANGE COLUMN `commonValue6` `commonValue6` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `commonValue5`;