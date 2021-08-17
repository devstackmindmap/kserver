CREATE TABLE `user_info` (
	`userid` INT(10) UNSIGNED NOT NULL,
	`isAlreadyFreeNicknameChange` TINYINT(4) NULL DEFAULT 0,
	`recentDateTimeCountryChange` DATETIME NULL DEFAULT '2010-01-01 00:00:00',
	`dailyRankVictoryGoldRewardCount` TINYINT(4) NULL DEFAULT 0,
	`dailyRankVictoryGoldRewardDateTime` DATETIME NULL DEFAULT '2010-01-01 00:00:00',
	`rewardedRankSeason` INT(11) NULL DEFAULT 0,
	PRIMARY KEY (`userid`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
ROW_FORMAT=DYNAMIC
;
