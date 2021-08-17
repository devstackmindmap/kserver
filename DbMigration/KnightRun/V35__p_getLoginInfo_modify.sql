DROP PROCEDURE `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
    IN `$areaNum` INT,
	OUT `$isFirstMember` BIT

)
BEGIN
	SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName, areaNum) VALUES($nickName, $areaNum);
		SET @userId = LAST_INSERT_ID();
		SET $isFirstMember = 1;

		SELECT userId, areaNum FROM accounts WHERE userId = @userId;
	ELSE 
		SET $isFirstMember = 0;		
		SELECT userId, areaNum FROM accounts WHERE nickName = $nickName;
	END IF;
END|
DELIMITER ;