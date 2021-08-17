ALTER TABLE `_ip2location`
	ADD COLUMN `group_code` TINYINT NULL DEFAULT NULL AFTER `country_name`;
	
update _ip2location set group_code = 1 where country_code = 'KR';
update _ip2location set group_code = 2 where country_code = 'CN';
update _ip2location set group_code = 3 where country_code = 'JP';
update _ip2location set group_code = 4 where country_code not in ('KR','CN','JP');


ALTER TABLE `_ip2location_ipv6`
	ADD COLUMN  `group_code` TINYINT NULL DEFAULT NULL AFTER `country_name`;
	
update _ip2location_ipv6 set group_code = 1 where country_code = 'KR';
update _ip2location_ipv6 set group_code = 2 where country_code = 'CN';
update _ip2location_ipv6 set group_code = 3 where country_code = 'JP';
update _ip2location_ipv6 set group_code = 4 where country_code not in ('KR','CN','JP');
