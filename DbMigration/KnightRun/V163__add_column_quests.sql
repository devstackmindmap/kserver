ALTER TABLE `quests`
	ADD COLUMN `dynamicQuestId` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `questType`;