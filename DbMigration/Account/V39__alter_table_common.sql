ALTER TABLE `_common`
	ADD COLUMN `commonId` INT NOT NULL FIRST,
	ADD PRIMARY KEY (`commonId`);

UPDATE _common SET commonId=1;