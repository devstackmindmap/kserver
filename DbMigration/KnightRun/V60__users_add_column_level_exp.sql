
ALTER TABLE `users`
	ADD COLUMN `level` INT(11) UNSIGNED NOT NULL DEFAULT '1' COMMENT '유저 레벨' AFTER `soPoint`,
	ADD COLUMN `exp` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0' COMMENT '현재 레벨에서의 경험치' AFTER `level`;