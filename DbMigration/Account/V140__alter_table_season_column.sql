ALTER TABLE `challenge_stage_first_clear_user`
	CHANGE COLUMN `season` `season` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `userId`;