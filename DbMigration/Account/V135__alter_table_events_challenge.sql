ALTER TABLE `_events`
	DROP INDEX `IX_events_startDateTime`,
	ADD INDEX `IX_events_startDateTime` (`eventType`, `endDateTime`, `startDateTime`);