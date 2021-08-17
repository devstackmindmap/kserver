ALTER TABLE `inprogress_stage`
	ADD COLUMN `treasureIdList` VARCHAR(150) NOT NULL AFTER `replaceCardStatIdList`,
	ADD COLUMN `proposalTreasureIdList` VARCHAR(150) NOT NULL AFTER `treasureIdList`;