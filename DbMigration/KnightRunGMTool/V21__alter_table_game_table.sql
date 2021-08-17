ALTER TABLE `game_table`
	ALTER `runMode` DROP DEFAULT;
ALTER TABLE `game_table`
	CHANGE COLUMN `runMode` `runMode` VARCHAR(20) NOT NULL AFTER `name`;