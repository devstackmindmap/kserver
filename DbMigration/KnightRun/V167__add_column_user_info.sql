ALTER TABLE `user_info`
	ADD COLUMN `lastRefreshDate` DATETIME NOT NULL DEFAULT '2010-01-01 00:00:00' AFTER `dailyQuestAddcount`;