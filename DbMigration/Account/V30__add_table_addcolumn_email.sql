ALTER TABLE `accounts`
	ADD COLUMN `email` VARCHAR(50) NOT NULL AFTER `socialAccount`;

UPDATE accounts SET email = userId;

ALTER TABLE `accounts`
	ADD UNIQUE INDEX `UX_accounts_email` (`email`);


CREATE TABLE `emailList` (
	`email` VARCHAR(50) NOT NULL,
	PRIMARY KEY (`email`)
)
COLLATE='utf8_general_ci'
;