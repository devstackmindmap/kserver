ALTER TABLE `user_info`
	ADD COLUMN `addDeck` INT NOT NULL DEFAULT '0' AFTER `lastRefreshDate`;