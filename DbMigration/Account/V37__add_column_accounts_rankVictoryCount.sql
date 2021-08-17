ALTER TABLE `accounts`
	ADD COLUMN `rankVictoryCount` INT(11) NOT NULL DEFAULT 0 AFTER `maxRankPoint`;