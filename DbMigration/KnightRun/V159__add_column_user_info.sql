ALTER TABLE `user_info`
	ADD COLUMN `enablePassList` VARCHAR(200) NOT NULL DEFAULT '0/' AFTER `unlockContents`;