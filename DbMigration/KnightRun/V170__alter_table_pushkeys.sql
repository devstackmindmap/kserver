ALTER TABLE `pushkeys`
	CHANGE COLUMN `pushAgree` `pushAgree` INT NOT NULL DEFAULT 0 AFTER `pushKey`,
	CHANGE COLUMN `nightPushAgree` `nightPushAgree` INT NOT NULL DEFAULT 0 AFTER `pushAgreeDatetime`;