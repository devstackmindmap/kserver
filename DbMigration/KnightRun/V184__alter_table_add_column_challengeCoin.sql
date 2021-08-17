ALTER TABLE `users`
	ADD COLUMN `challengeCoin` INT(11) NOT NULL DEFAULT 0 AFTER `soStartTicket`;