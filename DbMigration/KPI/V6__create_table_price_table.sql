-- --------------------------------------------------------
-- 호스트:                          knightrun.cluster-ro-ceofckii5eec.ap-northeast-2.rds.amazonaws.com
-- 서버 버전:                        5.7.12 - MySQL Community Server (GPL)
-- 서버 OS:                        Linux
-- HeidiSQL 버전:                  9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 테이블 kpi.price_table 구조 내보내기
CREATE TABLE IF NOT EXISTS `price_table` (
  `packagename` varchar(50) DEFAULT NULL,
  `storId` varchar(50) DEFAULT NULL,
  `gd_amount` int(11) DEFAULT NULL,
  `rm_amount` int(11) DEFAULT NULL,
  `price` int(11) DEFAULT NULL,
  `salePercent` float DEFAULT NULL,
  `GD_price` float DEFAULT NULL,
  `RM_price` float DEFAULT NULL,
  KEY `storId` (`storId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 테이블 데이터 kpi.price_table:~80 rows (대략적) 내보내기
/*!40000 ALTER TABLE `price_table` DISABLE KEYS */;
INSERT INTO `price_table` (`packagename`, `storId`, `gd_amount`, `rm_amount`, `price`, `salePercent`, `GD_price`, `RM_price`) VALUES
	('레어 메탈(소)', 'aos_raremetal_1', 0, 200, 3900, 1, 0, 19.5),
	('레어 메탈(중)', 'aos_raremetal_2', 0, 550, 11000, 1.1, 0, 18.2),
	('레어 메탈(중)', 'aos_raremetal_2_new', 0, 570, 11000, 1.12, 0, 18.2),
	('레어 메탈(대)', 'aos_raremetal_3', 0, 2900, 52000, 1.12, 0, 17.9),
	('레어 메탈(특대)', 'aos_raremetal_4', 0, 6200, 110000, 5.1, 0, 17.9),
	('초보자 패키지', 'aos_beginner_package', 3000, 600, 5900, 3.8, 1, 3.9),
	('런칭 기념 패키지', 'aos_lunching_package', 2500, 1000, 11000, 2.1, 1.3, 5.3),
	('마스터 나이트 패키지', 'aos_master_knight_package', 6000, 2500, 55000, 2.6, 2.4, 9.4),
	('AB소드 패키지', 'aos_ab_sword_package', 5000, 2500, 33000, 1, 1.9, 7.8),
	('얼리버드 패키지', 'coupone', 600, 150, 15000, 1, 5, 19.8),
	('슈퍼 얼리버드 패키지', 'coupone', 3000, 1400, 60000, 1, 4.9, 19.6),
	('울트라 얼리버드 패키지', 'coupone', 6000, 3000, 120000, 4.1, 5.1, 20.4),
	('5레벨 달성 패키지', 'aos_user_level_5_package', 1400, 400, 5900, 3.9, 1.2, 4.9),
	('10레벨 달성 패키지', 'aos_user_level_10_package', 3000, 500, 11000, 3.9, 1.3, 5.1),
	('15레벨 달성 패키지', 'aos_user_level_15_package', 3000, 500, 11000, 4.1, 1.3, 5.1),
	('20레벨 달성 패키지', 'aos_user_level_20_package', 8000, 2000, 22000, 4.1, 1.2, 4.9),
	('25레벨 달성 패키지', 'aos_user_level_25_package', 8000, 2000, 22000, 4, 1.2, 4.9),
	('30레벨 달성 패키지', 'aos_user_level_30_package', 11000, 3000, 33000, 4, 1.2, 5),
	('40레벨 달성 패키지', 'aos_user_level_40_package', 11000, 3000, 33000, 3, 1.2, 5),
	('60레벨 달성 패키지', 'aos_user_level_60_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('80레벨 달성 패키지', 'aos_user_level_80_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('100레벨 달성 패키지', 'aos_user_level_100_package', 0, 3000, 55000, 4.1, 0, 6.8),
	('교육생 패키지', 'aos_medal_01_package', 1400, 400, 5900, 3.9, 1.2, 4.9),
	('견습기사 패키지', 'aos_medal_02_package', 3000, 500, 11000, 4, 1.3, 5.1),
	('정식기사 패키지', 'aos_medal_03_package', 11000, 3000, 33000, 3, 1.2, 5),
	('베테랑 패키지', 'aos_medal_04_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('제로 브레이커 패키지', 'aos_medal_05_package', 15000, 3000, 55000, 4, 1.7, 6.8),
	('스킬 업 패키지', 'aos_skillup_01_package', 15000, 600, 33000, 6.2, 1.2, 5),
	('초급 전술관 유닛 패키지', 'aos_new_tactician_01_package', 0, 220, 1200, 6.2, 0, 3.2),
	('초급 전술관 스킬 패키지', 'aos_new_tactician_02_package', 0, 220, 1200, 4, 0, 3.2),
	('중급 전술관 유닛 패키지', 'aos_new_tactician_03_package', 550, 350, 3900, 4, 1.2, 5),
	('중급 전술관 스킬 패키지', 'aos_new_tactician_04_package', 550, 350, 3900, 3, 1.2, 5),
	('고급 전술관 패키지', 'aos_new_tactician_05_package', 900, 600, 7500, 3, 1.7, 6.7),
	('최고급 전술관 패키지', 'aos_new_tactician_06_package', 1000, 1400, 11000, 2.2, 1.7, 6.7),
	('GD 패키지', 'aos_gd_01_package', 4500, 100, 11000, 2.2, 2.2, 9),
	('나이트 패스', 'aos_knight_pass_01', 0, 0, 0, 2.3, 0, 0),
	('RM패키지', 'aos_rm_01_package', 600, 1100, 11000, 0, 2.2, 8.8),
	('고급 RM 패키지', 'aos_rm_02_package', 2000, 5000, 36000, 3.1, 1.6, 6.5),
	('고급 GD 패키지', 'aos_gd_02_package', 20000, 500, 36000, 1, 1.6, 6.5),
	('레니 패키지', 'aos_leny_01_package', 2000, 500, 99000, 1, 5, 20),
	('레어 메탈(소)', 'ios_raremetal_1', 0, 200, 3900, 1, 0, 19.5),
	('레어 메탈(중)', 'ios_raremetal_2', 0, 550, 11000, 1.1, 0, 18.2),
	('레어 메탈(중)', 'ios_raremetal_2_new', 0, 570, 11000, 1.12, 0, 18.2),
	('레어 메탈(대)', 'ios_raremetal_3', 0, 2900, 52000, 1.12, 0, 17.9),
	('레어 메탈(특대)', 'ios_raremetal_4', 0, 6200, 110000, 5.1, 0, 17.9),
	('초보자 패키지', 'ios_beginner_package', 3000, 600, 5900, 3.8, 1, 3.9),
	('런칭 기념 패키지', 'ios_lunching_package', 2500, 1000, 11000, 2.1, 1.3, 5.3),
	('마스터 나이트 패키지', 'ios_master_knight_package', 6000, 2500, 55000, 2.6, 2.4, 9.4),
	('AB소드 패키지', 'ios_ab_sword_package', 5000, 2500, 33000, 1, 1.9, 7.8),
	('얼리버드 패키지', 'coupon', 600, 150, 15000, 1, 5, 19.8),
	('슈퍼 얼리버드 패키지', 'coupon', 3000, 1400, 60000, 1, 4.9, 19.6),
	('울트라 얼리버드 패키지', 'coupon', 6000, 3000, 120000, 4.1, 5.1, 20.4),
	('5레벨 달성 패키지', 'ios_user_level_5_package', 1400, 400, 5900, 3.9, 1.2, 4.9),
	('10레벨 달성 패키지', 'ios_user_level_10_package', 3000, 500, 11000, 3.9, 1.3, 5.1),
	('15레벨 달성 패키지', 'ios_user_level_15_package', 3000, 500, 11000, 4.1, 1.3, 5.1),
	('20레벨 달성 패키지', 'ios_user_level_20_package', 8000, 2000, 22000, 4.1, 1.2, 4.9),
	('25레벨 달성 패키지', 'ios_user_level_25_package', 8000, 2000, 22000, 4, 1.2, 4.9),
	('30레벨 달성 패키지', 'ios_user_level_30_package', 11000, 3000, 33000, 4, 1.2, 5),
	('40레벨 달성 패키지', 'ios_user_level_40_package', 11000, 3000, 33000, 3, 1.2, 5),
	('60레벨 달성 패키지', 'ios_user_level_60_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('80레벨 달성 패키지', 'ios_user_level_80_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('100레벨 달성 패키지', 'ios_user_level_100_package', 0, 3000, 55000, 4.1, 0, 6.8),
	('교육생 패키지', 'ios_medal_01_package', 1400, 400, 5900, 3.9, 1.2, 4.9),
	('견습기사 패키지', 'ios_medal_02_package', 3000, 500, 11000, 4, 1.3, 5.1),
	('정식기사 패키지', 'ios_medal_03_package', 11000, 3000, 33000, 3, 1.2, 5),
	('베테랑 패키지', 'ios_medal_04_package', 15000, 3000, 55000, 3, 1.7, 6.8),
	('제로 브레이커 패키지', 'ios_medal_05_package', 15000, 3000, 55000, 4, 1.7, 6.8),
	('스킬 업 패키지', 'ios_skillup_01_package', 15000, 600, 33000, 6.2, 1.2, 5),
	('초급 전술관 유닛 패키지', 'ios_new_tactician_01_package', 0, 220, 1200, 6.2, 0, 3.2),
	('초급 전술관 스킬 패키지', 'ios_new_tactician_02_package', 0, 220, 1200, 4, 0, 3.2),
	('중급 전술관 유닛 패키지', 'ios_new_tactician_03_package', 550, 350, 3900, 4, 1.2, 5),
	('중급 전술관 스킬 패키지', 'ios_new_tactician_04_package', 550, 350, 3900, 3, 1.2, 5),
	('고급 전술관 패키지', 'ios_new_tactician_05_package', 900, 600, 7500, 3, 1.7, 6.7),
	('최고급 전술관 패키지', 'ios_new_tactician_06_package', 1000, 1400, 11000, 2.2, 1.7, 6.7),
	('GD 패키지', 'ios_gd_01_package', 4500, 100, 11000, 2.2, 2.2, 9),
	('나이트 패스', 'ios_knight_pass_01', 0, 0, 0, 2.3, 0, 0),
	('RM패키지', 'ios_rm_01_package', 600, 1100, 11000, 0, 2.2, 8.8),
	('고급 RM 패키지', 'ios_rm_02_package', 2000, 5000, 36000, 3.1, 1.6, 6.5),
	('고급 GD 패키지', 'ios_gd_02_package', 20000, 500, 36000, 1, 1.6, 6.5),
	('레니 패키지', 'ios_leny_01_package', 2000, 500, 99000, 1, 5, 20);
/*!40000 ALTER TABLE `price_table` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
