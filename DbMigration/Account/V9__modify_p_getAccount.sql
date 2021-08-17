DROP PROCEDURE IF EXISTS `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	IN `$loginDateTime` VARCHAR(20),
	IN `$distributeCode` VARCHAR(6),
	OUT `$isFirstMember` BIT,
	OUT `$isAllowCode` BIT,
	OUT `$outUserId` INT
)
BEGIN
	SELECT COUNT(distributeCode) FROM distribute_code WHERE distributeCode = $distributeCode INTO @isExistCode;
	IF @isExistCode = 1 THEN
		SET $isAllowCode = 1;

		SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
		IF @isExists = 0 THEN
			INSERT INTO accounts(nickName, loginDateTime) VALUES($nickName, $loginDateTime);
			SET @userId = LAST_INSERT_ID();
			SET $isFirstMember = 1;

			SELECT userId FROM accounts WHERE userId = @userId INTO $outUserId;
		ELSE 
			SET $isFirstMember = 0;
			SELECT userId FROM accounts WHERE nickName = $nickName INTO $outUserId;
		END IF;
	ELSE
		SET $isAllowCode = 0;
		SET $isFirstMember = 0;
		SET $outUserId = 0;
	END IF;
END|
DELIMITER ;