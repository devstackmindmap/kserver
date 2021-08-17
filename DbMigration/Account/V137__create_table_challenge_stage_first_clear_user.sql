CREATE TABLE `challenge_stage_first_clear_user` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NULL DEFAULT NULL,
	`season` INT(11) NULL DEFAULT NULL,
	`day` INT(11) NULL DEFAULT NULL,
	`difficultLevel` INT(11) NULL DEFAULT NULL,
	`insertDateTime` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_season_day_difficultLevel` (`season`, `day`, `difficultLevel`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
