
CREATE TABLE `knightrun_gmtool`.`account` (
  `username` varchar(10) NOT NULL,
  `password` varchar(20) NOT NULL,
  `grade` varchar(20) NOT NULL DEFAULT 'normal' COMMENT 'normal, manager, director, root',
  `executeMode` varchar(10) NOT NULL DEFAULT 'all' COMMENT 'all, get, post',
  PRIMARY KEY (`username`)
) ENGINE=InnoDB;
