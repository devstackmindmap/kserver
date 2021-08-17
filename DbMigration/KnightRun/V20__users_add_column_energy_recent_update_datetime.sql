ALTER TABLE `users`
	ADD COLUMN `energy_recent_update_datetime` DATETIME NULL DEFAULT NULL AFTER `bonus_energy`;