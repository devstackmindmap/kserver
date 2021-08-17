CREATE TABLE `daily_square_boxopen` (
	`ActivatedLevel` INT(11) NULL DEFAULT NULL,
	`BoxLevel` INT(11) NULL DEFAULT NULL,
	`LogType` INT(11) NULL DEFAULT NULL,
	`Count` INT(11) NOT NULL,
	`Date` DATE NOT NULL
)
COMMENT='스퀘어 오브젝트 박스 오픈 시 난이도, 박스 레벨, 로그타입 (1 스탑해서 오픈한 정보, 2 파괴), 오픈횟수, 날짜\r\n'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `daily_square_energy` (
	`CoreLevel` INT(11) NULL DEFAULT NULL,
	`AgencyLevel` INT(11) NULL DEFAULT NULL,
	`TotalBoxEnergy` INT(11) NULL DEFAULT NULL,
	`TotalBoxLevel` INT(11) NULL DEFAULT NULL,
	`TotalPowerEnergy` INT(11) NULL DEFAULT NULL,
	`TotalPower` INT(11) NULL DEFAULT NULL,
	`AvgPower` INT(11) NULL DEFAULT NULL,
	`AvgBoxLevel` INT(11) NULL DEFAULT NULL,
	`AvgBoxEnergy` INT(11) NULL DEFAULT NULL,
	`MaxPower` INT(11) NULL DEFAULT NULL,
	`MaxBoxLevel` INT(11) NULL DEFAULT NULL,
	`Count` INT(11) NULL DEFAULT NULL,
	`Date` DATE NULL DEFAULT NULL
)
COMMENT='스퀘어 오브젝트 에너지 주입 정보 \r\n'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `daily_square_start` (
	`MonsterLevel` INT(11) NOT NULL,
	`MonsterId` INT(11) NOT NULL,
	`ObjectLevel` INT(11) NOT NULL,
	`ActiveLevel` INT(11) NOT NULL,
	`CoreLevel` INT(11) NOT NULL,
	`AgencyLevel` INT(11) NOT NULL,
	`Count` INT(11) NOT NULL,
	`Date` DATE NOT NULL
)
COMMENT='스퀘어 오브젝트 시작 정보 \r\n'
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
