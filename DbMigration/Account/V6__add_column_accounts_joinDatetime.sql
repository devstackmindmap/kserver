ALTER TABLE `accounts`
	ADD COLUMN `joinDateTime` DATETIME NULL DEFAULT NULL AFTER `loginDateTime`;

DROP PROCEDURE IF EXISTS `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$nickName` VARCHAR(20),
	IN `$loginDateTime` VARCHAR(20),
	OUT `$isFirstMember` BIT,
	OUT `$userId` INT

)
BEGIN
	SELECT COUNT(nickName) FROM accounts WHERE nickName = $nickName INTO @isExists;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName, loginDateTime, joinDateTime) VALUES($nickName, $loginDateTime, $loginDateTime);
		SET @userId = LAST_INSERT_ID();
		SET $isFirstMember = 1;
	ELSE 
		SET $isFirstMember = 0;	
		SELECT userId FROM accounts WHERE nickName = $nickName INTO @userId;
		UPDATE accounts SET loginDateTime = $loginDateTime WHERE userId = @userId;	
	END IF;
	SET $userId = @userId;
END|
DELIMITER ;