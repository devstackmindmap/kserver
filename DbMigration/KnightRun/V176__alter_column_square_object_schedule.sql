ALTER TABLE `square_object_schedule`	
	ADD COLUMN `extraCoreEnergy` INT(11)  NOT NULL DEFAULT '0' AFTER `coreEnergy`,
	ADD COLUMN `extraEnergyInjectedTime` DATETIME NOT NULL DEFAULT '2010-01-01 00:00:00' AFTER `energyRefreshTime`,
	ADD COLUMN `enableReward` INT(11) NOT NULL DEFAULT '0' AFTER `objectExp`;
