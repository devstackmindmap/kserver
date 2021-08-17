ALTER TABLE `units`
	ADD COLUMN `currentVirtualRankLevel` INT(11) UNSIGNED NOT NULL DEFAULT '1' AFTER `nextSeasonRankPoint`,
	ADD COLUMN `currentVirtualRankPoint` INT(11) NOT NULL DEFAULT '0' AFTER `currentVirtualRankLevel`;