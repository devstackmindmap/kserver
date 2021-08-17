ALTER TABLE `user_info`
	ADD COLUMN `unlockContents` VARCHAR(100) NOT NULL DEFAULT '0/' AFTER `rewardedRankSeason`;
