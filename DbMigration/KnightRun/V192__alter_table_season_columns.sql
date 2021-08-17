ALTER TABLE `challenge_first_clear_decks`
	CHANGE COLUMN `season` `season` INT(11) UNSIGNED NOT NULL DEFAULT 0 AFTER `seq`;

ALTER TABLE `challenge_stage`
	CHANGE COLUMN `season` `season` INT(10) UNSIGNED NULL DEFAULT NULL AFTER `userId`;

ALTER TABLE `get_seasonpass`
	CHANGE COLUMN `season` `season` INT(11) UNSIGNED NULL DEFAULT NULL AFTER `seasonPassType`;