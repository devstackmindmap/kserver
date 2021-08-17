ALTER TABLE `push_log`
	CHANGE COLUMN `pushCastId` `pushCastId` VARCHAR(36) NOT NULL AFTER `seq`;
