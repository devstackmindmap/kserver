ALTER TABLE `knightrun_gmtool`.`game_table`
  MODIFY COLUMN `runMode` varchar(10) NOT NULL
  , MODIFY COLUMN `comment` text;

ALTER TABLE `knightrun_gmtool`.`server_list`
  MODIFY COLUMN `runMode` varchar(10) NOT NULL,
  DROP INDEX `IX_serverList_runMode_ip`; 


