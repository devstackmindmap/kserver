ALTER TABLE `battle_records`
	CHANGE COLUMN `recordKey` `recordKey` VARCHAR(50) NOT NULL DEFAULT '0' AFTER `battleInfo`;