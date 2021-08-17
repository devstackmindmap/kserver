ALTER TABLE `battle_records`
	DROP COLUMN `isSaved`,
	DROP INDEX `IX_battle_records_player1UserId_battleType`,
	ADD INDEX `IX_battle_records_player1UserId_battleType` (`userId`, `battleEndTime`, `battleType`);
