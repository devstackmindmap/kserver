CREATE TABLE `challenge_first_clear_decks_event` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`challengeEventId` INT(11) UNSIGNED NOT NULL DEFAULT 0,
	`difficultLevel` INT(11) NOT NULL DEFAULT 0,
	`userId` INT(11) UNSIGNED NOT NULL,
	`modeType` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0,
	`slotType` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0,
	`deckNum` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0,
	`orderNum` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0,
	`classId` INT(11) UNSIGNED NOT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_challenge_first_clear_decks_event_eventChallengeId` (`challengeEventId`, `difficultLevel`, `userId`)
)
COMMENT='Deck Info'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
