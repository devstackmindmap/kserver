ALTER TABLE `units`
	ADD COLUMN `maxVirtualRankLevel` INT UNSIGNED NOT NULL DEFAULT '1' AFTER `nextSeasonRankPoint`;