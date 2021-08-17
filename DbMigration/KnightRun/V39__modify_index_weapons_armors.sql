ALTER TABLE `weapons`
	DROP INDEX `IX_weapons_unitId`,
	ADD INDEX `IX_weapons_userId_unitId` (`userId`, `unitId`);

ALTER TABLE `armors`
	DROP INDEX `IX_armors_unitId`,
	ADD INDEX `IX_armors_userId_unitId` (`userId`, `unitId`);