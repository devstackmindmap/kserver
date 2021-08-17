DROP TABLE  IF EXISTS emaillist;

ALTER TABLE `accounts`
	DROP COLUMN  email,
	DROP INDEX UX_accounts_email;

