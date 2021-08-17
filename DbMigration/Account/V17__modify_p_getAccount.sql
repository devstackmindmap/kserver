DROP PROCEDURE IF EXISTS `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	IN `$loginDateTime` VARCHAR(20),
	IN `$distributeCode` VARCHAR(6),
	OUT `$isFirstMember` BIT,
	OUT `$isAllowCode` BIT,
	OUT `$outUserId` INT,
	OUT `$outInitDataVersion` INT
)
BEGIN
	SELECT COUNT(distributeCode), seq FROM distribute_code WHERE distributeCode = $distributeCode INTO @isExistCode, @seq;
	IF @isExistCode = 1 THEN
		SET $isAllowCode = 1;

		SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
		IF @isExists = 0 THEN
			INSERT INTO accounts(nickName, loginDateTime, joinDateTime, distributeCodeSeq) 
            VALUES($nickName, $loginDateTime, $loginDateTime, @seq);
			SET @userId = LAST_INSERT_ID();
			SET $isFirstMember = 1;
			SET $outInitDataVersion = 0;

			SELECT userId FROM accounts WHERE userId = @userId INTO $outUserId;
		ELSE 
			SET $isFirstMember = 0;
			SELECT userId, initDataVersion  FROM accounts WHERE nickName = $nickName INTO $outUserId, $outInitDataVersion;
			UPDATE accounts SET loginDateTime = $loginDateTime WHERE userId = $outUserId;
		END IF;
	ELSE
		SET $isAllowCode = 0;
		SET $isFirstMember = 0;
		SET $outUserId = 0;
		SET $outInitDataVersion = 0;
	END IF;
END|
DELIMITER ;