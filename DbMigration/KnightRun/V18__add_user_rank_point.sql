DROP PROCEDURE `p_armorLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_armorLevelUp`(
	IN `$userId` INT,
	IN `$pieceId` INT,
	IN `$goldCount` INT,
	IN `$pieceCount` INT
	
)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
	START TRANSACTION;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE armors SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
	COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE `p_cardLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_cardLevelUp`(
	IN `$userId` INT,
	IN `$pieceId` INT,
	IN `$goldCount` INT,
	IN `$pieceCount` INT

)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
	START TRANSACTION;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE cards SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
	COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	OUT `$isFirstMember` BIT

)
BEGIN
	SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName) VALUES($nickName);
		SET @userId = LAST_INSERT_ID();
		SET $isFirstMember = 1;

		SELECT userId FROM accounts WHERE userId = @userId;
	ELSE 
		SET $isFirstMember = 0;		
		SELECT userId FROM accounts WHERE nickName = $nickName;
	END IF;
END|
DELIMITER ;

DROP PROCEDURE `p_getClearCountAfterStageLevelWin`;

DELIMITER |
CREATE PROCEDURE `p_getClearCountAfterStageLevelWin`(
	IN `$userId` INT,
	IN `$stageLevelId` INT

)
BEGIN
	INSERT INTO stage_levels (userId, stageLevelId, clearCount) VALUES ($userId, $stageLevelId, 1)
	ON DUPLICATE KEY UPDATE clearCount = clearCount + 1;
	SELECT clearCount FROM stage_levels WHERE userId = $userId AND stageLevelId=$stageLevelId;
END|
DELIMITER ;

DROP PROCEDURE `p_getLoginInfo`;

DELIMITER |
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT

)
BEGIN
	SELECT gold, gem FROM users WHERE userId = $userId;
	SELECT id, level, count, rankPoint FROM units WHERE userId = $userId;
	SELECT id, level, count FROM cards WHERE userId = $userId;
	SELECT id, level, count, unitId FROM weapons WHERE userId = $userId;
	SELECT id, level, count, unitId FROM armors WHERE userId = $userId;
	SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
	SELECT rewardUId, rewardType, classId, rewardOpenTimeTick FROM rewards WHERE userId = $userId;
END|
DELIMITER ;

DROP PROCEDURE `p_unitLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_unitLevelUp`(
	IN `$userId` INT,
	IN `$pieceId` INT,
	IN `$goldCount` INT,
	IN `$pieceCount` INT

)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
	START TRANSACTION;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE units SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
	COMMIT;
END$$
DELIMITER ;

DELIMITER |
CREATE PROCEDURE `p_updateUnitRankPoint`(
	IN `$userId` INT,
	IN `$unitId` INT,
	IN `$deltaRankPoint` INT,
	OUT `$out_rankPoint` INT
)
BEGIN
	SELECT @rankPoint := rankPoint FROM units WHERE userId = $userId AND id = $unitId;
	SET $out_rankPoint = @rankPoint + $deltaRankPoint;
	
	IF $out_rankPoint < 0 THEN
		SET $out_rankPoint = 0;
	END IF;
	
	UPDATE units SET rankPoint = $out_rankPoint WHERE userId = $userId AND id = $unitId;
END|
DELIMITER ;

DROP PROCEDURE `p_weaponLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_weaponLevelUp`(
	IN `$userId` INT,
	IN `$pieceId` INT,
	IN `$goldCount` INT,
	IN `$pieceCount` INT

)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		ROLLBACK;

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
  	
	START TRANSACTION;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE weapons SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
	COMMIT;
END$$
DELIMITER ;

