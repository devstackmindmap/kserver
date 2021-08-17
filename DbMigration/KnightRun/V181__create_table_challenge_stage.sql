CREATE TABLE `challenge_stage` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NULL DEFAULT NULL,
	`season` INT(10) NULL DEFAULT NULL,
	`day` INT(10) NULL DEFAULT NULL,
	`difficultLevel` INT(10) NULL DEFAULT NULL,
	`clearCount` INT(10) NULL DEFAULT 0,
	`isRewarded` INT(10) NULL DEFAULT 0,
	`rewardResetCount` INT(10) NULL DEFAULT 0,
	`inprogressRound` INT(10) NULL DEFAULT 0,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_challenge_stage_userId_season_day_difficultLevel` (`userId`, `season`, `day`, `difficultLevel`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
