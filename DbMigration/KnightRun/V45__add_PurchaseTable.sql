CREATE TABLE IF NOT EXISTS `purchase_item` (
  `sn` int(11) NOT NULL AUTO_INCREMENT,
  `productID` char(50) NOT NULL,
  `packageName` char(50) NOT NULL,
  `RewardType` int(11) NOT NULL,
  `RewardAmount` int(11) NOT NULL,
  PRIMARY KEY (`sn`),
  KEY `productID` (`productID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;


INSERT INTO `purchase_item` ( `productID`, `packageName`, `RewardType`, `RewardAmount`) VALUES
	( '1000won', '천원', 1, 10),
	( 'rarematal1000_30000won', '레어메탈30개', 1, 1000),
	( 'rarematal100_3000won', '레어메탈100개', 1, 100),
	( 'rarematal30_1000won', '레어메탈30개', 1, 30);
/*!40000 ALTER TABLE `purchase_item` ENABLE KEYS */;

-- 테이블 knightrun.purchase_log 구조 내보내기
CREATE TABLE IF NOT EXISTS `purchase_log` (
  `sn` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) NOT NULL,
  `productID` char(50) NOT NULL,
  `purchaseToken` varchar(1024) NOT NULL,
  `transactionId` varchar(1024) NOT NULL,
  `logTime` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`sn`),
  UNIQUE KEY `sn` (`sn`),
  KEY `UserID` (`UserID`),
  KEY `purchaseToken` (`purchaseToken`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;
