ALTER TABLE `_ip2location`
	ADD INDEX `IX_ip2location_country_code` (`country_code`);

ALTER TABLE `_ip2location_ipv6`
	ADD INDEX `IX_ip2location_ipv6_country_code` (`country_code`);