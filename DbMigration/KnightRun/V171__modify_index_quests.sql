
ALTER TABLE `quests`
	ADD INDEX `IX_quest_userId_questType` (`userId`, `questType`);
