ALTER TABLE `clan_members`
	ADD COLUMN `memberGrade` INT(10) NOT NULL DEFAULT 0 AFTER `userId`;


	