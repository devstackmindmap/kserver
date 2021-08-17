DROP PROCEDURE IF EXISTS `p_setDeck`;

DELIMITER $$
CREATE PROCEDURE `p_setDeck`(
	IN `$userId` INT,
	IN `$modeType` TINYINT,
	IN `$slotType` TINYINT,
	IN `$deckNum` TINYINT,
	IN `$orderNum` TINYINT,
	IN `$classId` INT

)
BEGIN  
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION	  
  	BEGIN
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;
	INSERT INTO decks(userId, modeType, slotType, deckNum, orderNum, classId) 
	VALUES($userId, $modeType, $slotType, $deckNum, $orderNum, $classId)
	ON DUPLICATE KEY UPDATE classId = $classId;
END$$
DELIMITER ;