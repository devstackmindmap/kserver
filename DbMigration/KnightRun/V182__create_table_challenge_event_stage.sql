CREATE TABLE `challenge_event_stage` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NULL DEFAULT NULL,
	`challengeEventId` INT(10) UNSIGNED NULL DEFAULT NULL,
	`difficultLevel` INT(10) NULL DEFAULT NULL,
	`clearCount` INT(10) NULL DEFAULT 0,
	`isRewarded` INT(10) NULL DEFAULT 0,
	`rewardResetCount` INT(10) NULL DEFAULT 0,
	`inprogressRound` INT(11) NULL DEFAULT 0,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_challenge_event_stage_userId_challengeEventId_difficultLevel` (`userId`, `challengeEventId`, `difficultLevel`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
