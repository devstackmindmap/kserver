
ALTER TABLE `accounts`
	ADD COLUMN `socialAccount` VARCHAR(50) NOT NULL AFTER `initDataVersion`;

UPDATE `accounts` SET socialAccount = nickName;

ALTER TABLE `accounts`
	ADD UNIQUE INDEX `UX_accounts_socialAccount` (`socialAccount`);