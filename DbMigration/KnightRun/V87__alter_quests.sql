ALTER TABLE `quests`
	CHANGE COLUMN `receivedReward` `receivedOrder` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `performCount`,
	CHANGE COLUMN `complete` `completedOrder` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `receivedOrder`,
	ADD COLUMN `questType` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `completedOrder`;

