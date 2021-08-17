ALTER TABLE `user_mail_private`
	CHANGE COLUMN `mailIcon` `mailIcon` VARCHAR(50) NOT NULL DEFAULT '0' AFTER `isRead`;