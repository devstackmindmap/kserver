DROP PROCEDURE IF EXISTS `p_getAccount`;

DELIMITER |
CREATE PROCEDURE `p_getAccount`(
	IN `$socialAccount` VARCHAR(50),
	IN `$nickName` VARCHAR(20),
	IN `$joinDateTime` VARCHAR(20),
	IN `$serverSeason` INT,
	IN `$email` VARCHAR(50),
	OUT `$outUserId` INT,
	OUT `$outInitDataVersion` INT,
	OUT `$outNicknameDuplicate` BIT
)
BEGIN
	SELECT COUNT(nickName) INTO @isExists FROM accounts WHERE nickName = $nickName;
	IF @isExists = 0 THEN
		INSERT INTO accounts(nickName, joinDateTime, currentSeason, socialAccount, email) 
		VALUES($nickName, $joinDateTime, $serverSeason, $socialAccount, $email);
		SET $outUserId = LAST_INSERT_ID();
		SET $outInitDataVersion = 0;
		SET $outNicknameDuplicate = 0;
	ELSE 
		SET $outNicknameDuplicate = 1;
	END IF;
END|
DELIMITER ;