
ALTER TABLE `accounts`
	ADD COLUMN `maxVirtualRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1' AFTER `rankVictoryCount`,
	ADD COLUMN `currentVirtualRankPoint` INT(11) NOT NULL DEFAULT '0' AFTER `maxVirtualRankLevel`,
	ADD COLUMN `maxVirtualRankPoint` INT(11) NULL DEFAULT '0' AFTER `currentVirtualRankPoint`;