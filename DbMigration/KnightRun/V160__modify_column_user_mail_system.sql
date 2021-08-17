ALTER TABLE `user_mail_system`
    CHANGE COLUMN `mailId` `systemMailId` INT(10) UNSIGNED NULL DEFAULT NULL AFTER `userId`;

ALTER TABLE `user_mail_system`
    CHANGE COLUMN `seq` `mailId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT FIRST;

ALTER TABLE `user_mail_system`
	DROP INDEX `UX_user_mail_system_userId_mailId`,
	ADD INDEX `IX_user_mail_system_userId_mailId` (`userId`, `systemMailId`);