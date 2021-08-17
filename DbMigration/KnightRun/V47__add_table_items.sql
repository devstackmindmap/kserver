CREATE TABLE `_items` (
	`seq` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
	`itemId` INT(11) UNSIGNED NOT NULL,
	`itemType` INT(11) NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`value` INT(11) NOT NULL,
	PRIMARY KEY (`seq`),
	INDEX `IX_items_itemId` (`itemId`)
)
COMMENT='_store 테이블에서 지정된 item'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
