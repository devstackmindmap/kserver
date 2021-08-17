ALTER TABLE `user_info`
	ADD COLUMN `maxVirtualRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1' AFTER `unlockContents`,
	ADD COLUMN `currentVirtualRankPoint` INT(11) NOT NULL DEFAULT '0' AFTER `maxVirtualRankLevel`,
	ADD COLUMN `maxVirtualRankPoint` INT(11) NOT NULL DEFAULT '0' AFTER `currentVirtualRankPoint`;