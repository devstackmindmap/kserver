ALTER TABLE `_ip2location`
	CHANGE COLUMN `country_code` `country_code` CHAR(2) NULL DEFAULT NULL COLLATE 'utf8_general_ci' AFTER `ip_to`,
	CHANGE COLUMN `country_name` `country_name` VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_general_ci' AFTER `country_code`,
	CHANGE COLUMN `group_code` `group_code` INT NULL DEFAULT NULL AFTER `country_name`;

ALTER TABLE `_ip2location_ipv6`
	CHANGE COLUMN `group_code` `group_code` INT NULL DEFAULT NULL AFTER `country_name`;


UPDATE _ip2location SET country_code='KR', country_name='Korea, Republic of', group_code=1 WHERE country_code='-0';
UPDATE _ip2location_ipv6 SET country_code='KR', country_name='Korea, Republic of',  group_code=1 WHERE country_code='-0';