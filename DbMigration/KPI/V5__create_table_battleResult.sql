

CREATE TABLE `deckset` (
	`units` VARCHAR(30) NOT NULL,
	`cards` VARCHAR(50) NOT NULL,
	`wincount` INT(11) NOT NULL DEFAULT '0',
	`losecount` INT(11) NOT NULL DEFAULT '0',
	`date` DATE NOT NULL,
	UNIQUE INDEX `units_cards_winlose_date` (`units`, `cards`, `date`)
)
COMMENT='덱 구성에 따른 승패 정보.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;



CREATE TABLE `winlose_unit_count` (
	`unitId` INT(11) NOT NULL DEFAULT '0',
	`Level` INT(11) NOT NULL DEFAULT '0',
	`wincount` INT(11) NOT NULL DEFAULT '0',
	`losecount` INT(11) NOT NULL DEFAULT '0',
	`Date` DATE NOT NULL,
	UNIQUE INDEX `unitId_Level_Date` (`unitId`, `Level`, `Date`),
	INDEX `unitId` (`unitId`)
)
COMMENT='유닛 레벨과 승패 카운트.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;



CREATE TABLE `winlose_card_count` (
	`cardId` INT(11) NOT NULL DEFAULT '0',
	`wincount` INT(11) NOT NULL DEFAULT '0',
	`losecount` INT(11) NOT NULL DEFAULT '0',
	`Date` DATE NOT NULL,
	UNIQUE INDEX `cardId_Date` (`cardId`, `Date`),
	INDEX `cardId` (`cardId`)
)
COMMENT='스킬에 따른 승패 정보.'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
