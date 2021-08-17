DROP PROCEDURE `p_unitLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_unitLevelUp`(
	IN `$userId` INT,
	IN `$pieceId` INT,
	IN `$goldCount` INT,
	IN `$pieceCount` INT


)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE units SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
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

  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE cards SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
END$$
DELIMITER ;

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
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE armors SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
END$$
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
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE weapons SET level = level + 1, count = count - $pieceCount WHERE userId = $userId AND id = $pieceId;
	
END$$
DELIMITER ;