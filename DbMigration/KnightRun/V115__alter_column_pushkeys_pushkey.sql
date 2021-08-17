ALTER TABLE `pushkeys`
	CHANGE COLUMN `pushKeys` `pushKey` VARCHAR(255) NOT NULL AFTER `userId`;