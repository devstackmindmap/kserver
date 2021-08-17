ALTER TABLE `users`
	ADD COLUMN `bonus_energy` INT(11) NOT NULL DEFAULT '0' AFTER `energy`;