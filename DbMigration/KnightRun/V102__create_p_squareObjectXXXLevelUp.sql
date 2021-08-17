DROP PROCEDURE  IF EXISTS `p_squareObjectLevelUp`;
DROP PROCEDURE  IF EXISTS `p_squareObjectCoreLevelUp`;
DROP PROCEDURE  IF EXISTS `p_squareObjectAgencyLevelUp`;

DELIMITER $$
CREATE PROCEDURE `p_squareObjectLevelUp`(
	IN $userId INT,
	IN $pieceId INT, 
	IN $goldCount INT, 
	IN $pieceCount INT
)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE square_object_schedule SET objectLevel = objectLevel + 1, objectExp = objectExp - $pieceCount WHERE userId = $userId ;
	
END$$
DELIMITER ;


DELIMITER $$
CREATE PROCEDURE `p_squareObjectCoreLevelUp`(
	IN $userId INT,
	IN $pieceId INT, 
	IN $goldCount INT, 
	IN $pieceCount INT
)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE square_object_schedule SET coreLevel = coreLevel + 1, coreExp = coreExp - $pieceCount WHERE userId = $userId ;
	
END$$
DELIMITER ;



DELIMITER $$
CREATE PROCEDURE `p_squareObjectAgencyLevelUp`(
	IN $userId INT,
	IN $pieceId INT, 
	IN $goldCount INT, 
	IN $pieceCount INT
)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 

		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
		
	UPDATE users SET gold = gold - $goldCount WHERE userId = $userId;
	UPDATE square_object_schedule SET agencyLevel = agencyLevel + 1, agencyExp = agencyExp - $pieceCount WHERE userId = $userId ;

END$$
DELIMITER ;

