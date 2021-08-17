CREATE TABLE `ad_view_log` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`ad_network` CHAR(50) NOT NULL,
	`ad_unit` CHAR(50) NOT NULL,
	`reward_amount` INT(11) NULL DEFAULT NULL,
	`reward_item` CHAR(50) NULL DEFAULT NULL,
	`timestamp` CHAR(50) NOT NULL,
	`transaction_id` CHAR(50) NOT NULL,
	`user_id` INT(11) NULL DEFAULT NULL,
	`signature` VARCHAR(256) NOT NULL,
	`key_id` CHAR(50) NOT NULL,
	`is_received` INT(11) NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	INDEX `user_id` (`user_id`),
	INDEX `transaction_id` (`transaction_id`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
