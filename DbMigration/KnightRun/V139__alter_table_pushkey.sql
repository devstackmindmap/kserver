ALTER TABLE `pushkeys`
	ADD COLUMN `termsAgree` BIT(1) NOT NULL AFTER `lastLoginDate`,
	ADD COLUMN `termsAgreeDatetime` DATETIME NOT NULL AFTER `termsAgree`;
