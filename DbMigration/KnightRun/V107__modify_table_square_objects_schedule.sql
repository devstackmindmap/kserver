ALTER TABLE `square_object_schedule`
	ADD COLUMN `nextInvasionMonsterId` INT UNSIGNED NOT NULL DEFAULT '0' AFTER `nextInvasionTime`,
	DROP COLUMN `hasReward`;