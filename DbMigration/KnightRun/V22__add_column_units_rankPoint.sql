﻿ALTER TABLE `units`
	ADD COLUMN `rankPoint` INT(11) NOT NULL DEFAULT '0' AFTER `count`;