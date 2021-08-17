CREATE TABLE IF NOT EXISTS `publicNotice` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`NoticeMessage` VARCHAR(120) NOT NULL DEFAULT '0',
	`RunMode` CHAR(10) NOT NULL DEFAULT '',
	`Count` TINYINT(3) UNSIGNED NOT NULL,
	`StartTime` DATETIME NOT NULL,
	PRIMARY KEY (`seq`),
	INDEX `Count_StartTime` (`Count`, `StartTime`)
)
COMMENT='공지 예약 테이블 '
COLLATE='utf8_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=1
;
