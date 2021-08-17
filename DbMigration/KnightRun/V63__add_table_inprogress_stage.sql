
DROP PROCEDURE  IF EXISTS `p_getClearCountAfterStageLevelWin`;

CREATE TABLE `inprogress_stage` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`battleType` INT(11) NOT NULL,
	`stageLevelId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`clearRound` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`cardStatIdList` VARCHAR(100) NOT NULL,
	`replaceCardStatIdList` VARCHAR(100) NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_inprogress_stage_userId_battleType` (`userId`, `battleType`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
