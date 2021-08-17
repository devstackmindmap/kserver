DELIMITER |
CREATE PROCEDURE `p_createClan`(
	IN `$userId` INT,
	IN `$clanName` VARCHAR(50),
	IN `$clanSymbolId` INT,
	IN `$clanPublicType` INT,
	IN `$joinConditionRankPoint` INT,
	OUT `$outResultCode` INT,
	OUT `$outClanId` INT,
	OUT `$outRankPoint` INT,
	OUT `$outNextRankPoint` INT
)
BEGIN
	SELECT count(userId) INTO @isExistClan FROM clan_members WHERE userId=$userId;
	IF @isExistClan = 1 THEN
		SET $outResultCode = 1;
	ELSE
		SELECT count(clanName) INTO @isExistClanName FROM clans WHERE clanName=$clanName;
		IF @isExistClanName = 1 THEN
			SET $outResultCode = 2;
		ELSE
			SELECT currentSeason, currentSeasonRankPoint, nextSeasonRankPoint INTO @currentSeason, @currentSeasonRankPoint, @nextSeasonRankPoint
			FROM accounts WHERE userId=$userId;

			INSERT INTO clans 
			(clanMaterUserId,clanName,clanSymbolId,clanPublicType,joinConditionRankPoint,currentSeason,currentSeasonRankPoint,nextSeasonRankPoint,memberCount)
			VALUES 
			($userId,$clanName,$clanSymbolId,$clanPublicType,$joinConditionRankPoint,@currentSeason,@currentSeasonRankPoint,@nextSeasonRankPoint,1);
			SET $outClanId = LAST_INSERT_ID();
			INSERT INTO clan_members (clanId, userId) VALUES ($outClanId, $userId);
			UPDATE accounts SET clanName=$clanName WHERE userId=$userId;
			SET $outResultCode = 0;
			SET $outRankPoint = @currentSeasonRankPoint;
			SET $outNextRankPoint = @nextSeasonRankPoint;
		END IF;
	END IF;
END|
DELIMITER ;