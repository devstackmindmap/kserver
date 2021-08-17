ALTER TABLE `weapons`
	DROP COLUMN `unitId`,
	DROP INDEX `IX_weapons_userId_unitId`;