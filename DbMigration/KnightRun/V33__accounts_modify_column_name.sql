ALTER TABLE `accounts`
  DROP COLUMN `lastLoginDate`
  , ADD COLUMN `loginDateTime` datetime NULL DEFAULT NULL;