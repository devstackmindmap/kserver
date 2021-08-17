
ALTER TABLE `asset_bundle`
	ADD COLUMN `bundleVersion` INT NOT NULL DEFAULT '0' AFTER `runMode`;
	
ALTER TABLE `game_table`
	ADD COLUMN `bundleVersion` INT NOT NULL DEFAULT '0' AFTER `runMode`;
