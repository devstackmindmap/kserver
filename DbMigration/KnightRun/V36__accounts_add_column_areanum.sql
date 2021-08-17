ALTER TABLE `accounts`
	ADD COLUMN `areaNum` INT NOT NULL AFTER `nickName`;

UPDATE accounts SET areaNum = 1;