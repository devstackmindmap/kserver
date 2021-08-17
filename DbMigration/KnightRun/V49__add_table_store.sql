CREATE TABLE `_store` (
	`storeId` INT(11) NOT NULL AUTO_INCREMENT,
	`storeType` INT(11) NOT NULL,
	`goodsImage` VARCHAR(50) NOT NULL,
	`text` VARCHAR(50) NOT NULL,
	`purchaseMaterialType` INT(11) NOT NULL,
	`cost` INT(11) NOT NULL,
	`itemId` INT(11) UNSIGNED NOT NULL,
	`scheduleId` INT(11) NOT NULL,
	PRIMARY KEY (`storeId`),
	INDEX `FK_store_itemId` (`itemId`),
	INDEX `FK_store__schedules` (`scheduleId`),
	CONSTRAINT `FK__store__schedules` FOREIGN KEY (`scheduleId`) REFERENCES `_schedules` (`scheduleId`),
	CONSTRAINT `FK_store_itemId` FOREIGN KEY (`itemId`) REFERENCES `_items` (`itemId`)
)
COMMENT='상점테이블'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
