ALTER TABLE `battle_records` 
	DROP COLUMN `behaviors`;

ALTER TABLE `battle_records`
	CHANGE COLUMN `player1UserId` `userId` INT(11) UNSIGNED NOT NULL AFTER `seq`,
	CHANGE COLUMN `battleType` `battleType` VARCHAR(50) NOT NULL DEFAULT '0' AFTER `userId`,
	CHANGE COLUMN `player2UserId` `enemyUserId` INT(11) UNSIGNED NOT NULL AFTER `battleType`,
	ADD COLUMN `nickName` VARCHAR(20) NULL AFTER `enemyUserId`,
	ADD COLUMN `enemyNickName` VARCHAR(20) NULL AFTER `nickName`,
	ADD COLUMN `winner` INT NOT NULL DEFAULT '0' AFTER `enemyNickName`,
	ADD COLUMN `battleStartTime` DATETIME NULL AFTER `winner`,
	ADD COLUMN `battleEndTime` DATETIME NULL AFTER `battleStartTime`,
    ADD COLUMN `behaviorsId` INT(11) UNSIGNED NOT NULL DEFAULT '0' AFTER `battleType`,
	ADD UNIQUE INDEX `UX_battle_records_playerUserId_behaviorsId` (`userId`, `behaviorsId`);