DROP PROCEDURE IF EXISTS `p_createClan`;

DELIMITER |
CREATE PROCEDURE `p_createClan`(
	IN `$userId` INT,
	IN `$clanName` VARCHAR(50),
	IN `$clanSymbolId` INT,
	IN `$clanPublicType` INT,
	IN `$joinConditionRankPoint` INT,
	IN `$countryCode` VARCHAR(2),
	IN `$clanExplain` VARCHAR(128),
	OUT `$outResultCode` INT,
	OUT `$outClanId` INT,
	OUT `$outRankPoint` INT
)
BEGIN

	SET $outResultCode = 0;

	SELECT clanId INTO @isExistClan FROM clan_members WHERE userId=$userId;
	IF @isExistClan IS NOT null THEN
		IF @isExistClan <> 0 THEN
			SET $outResultCode = 1;
		END IF;
	END IF;

	SELECT count(clanName) INTO @isExistClanName FROM clans WHERE clanName=$clanName;
	IF @isExistClanName = 1 THEN
		SET $outResultCode = 2;
	END IF;

	IF $outResultCode = 0 THEN
		SELECT currentSeasonRankPoint INTO @currentSeasonRankPoint
		FROM accounts WHERE userId=$userId;

		SET $outRankPoint = @currentSeasonRankPoint;

		INSERT INTO clans 
		(clanMasterUserId,clanName,clanSymbolId,clanPublicType,joinConditionRankPoint,rankPoint,memberCount,countryCode,clanExplain)
		VALUES 
		($userId,$clanName,$clanSymbolId,$clanPublicType,$joinConditionRankPoint,@currentSeasonRankPoint,1,$countryCode,$clanExplain);
		SET $outClanId = LAST_INSERT_ID();
		INSERT INTO clan_members (clanId, userId, memberGrade) VALUES ($outClanId, $userId, 1)
		ON DUPLICATE KEY UPDATE clanId = $outClanId, memberGrade = 1;
		UPDATE accounts SET clanName=$clanName WHERE userId=$userId;
	END IF;
END|
DELIMITER ;