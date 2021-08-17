DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	IN `$loginDateTime` VARCHAR(20),
	OUT `$isFirstMember` BIT

)
BEGIN
	SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName, loginDateTime) VALUES($nickName, $loginDateTime);
		SET @userId = LAST_INSERT_ID();
		SET $isFirstMember = 1;

		SELECT userId FROM accounts WHERE userId = @userId;
	ELSE 
		SET $isFirstMember = 0;		
		SELECT userId FROM accounts WHERE nickName = $nickName;
	END IF;
END|
DELIMITER ;