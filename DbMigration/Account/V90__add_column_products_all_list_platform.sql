ALTER TABLE `_products_all_list`
	ADD COLUMN `platform` INT(11) NOT NULL AFTER `endDateTime`;

ALTER TABLE `_products_all_list`
	DROP INDEX `IX_products_all_list_startDateTime_endDateTime`,
	ADD INDEX `IX_products_all_list_startDateTime_endDateTime` (`endDateTime`, `startDateTime`, `platform`);

UPDATE _products_all_list SET platform = 1;

ALTER TABLE `_products_event_real`
	ADD COLUMN `platform` INT(11) NOT NULL AFTER `endDateTime`;
UPDATE _products_event_real SET platform = 1;

ALTER TABLE `_products_event_real`
	DROP INDEX `IX_products_event_digital_startDateTime_endDateTime`,
	ADD INDEX `IX_products_event_digital_startDateTime_endDateTime` (`endDateTime`, `startDateTime`, `platform`);

	
ALTER TABLE `_products_fix_real`
	ADD COLUMN `platform` INT(11) NOT NULL AFTER `endDateTime`;
UPDATE _products_fix_real SET platform = 1;

ALTER TABLE `_products_fix_real`
	ADD INDEX `IX_products_fix_real_platform` (`platform`);