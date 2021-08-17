ALTER TABLE `user_info`
	ADD COLUMN `dailyQuestRefreshCount` INT(11)  NOT NULL DEFAULT '0' AFTER `maxVirtualRankPoint`,
	ADD COLUMN `dailyQuestAddcount` INT(11) NOT NULL DEFAULT '0' AFTER `dailyQuestRefreshCount`;