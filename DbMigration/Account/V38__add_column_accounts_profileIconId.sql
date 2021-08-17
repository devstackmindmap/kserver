ALTER TABLE `accounts`
	ADD COLUMN `profileIconId` INT(11) NOT NULL DEFAULT 1 AFTER `rankVictoryCount`;

UPDATE accounts SET profileIconId = 1;