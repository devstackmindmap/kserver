DROP PROCEDURE IF EXISTS `p_joinClan`;
DELIMITER |
CREATE PROCEDURE `p_joinClan`(
	IN `$userId` INT,
	IN `$clanId` INT,
	IN `$isConditionCheck` INT,
	IN `$fullMemberCount` INT,
	OUT `$outResultCode` INT,
	OUT `$outUserRankPoint` INT,
	OUT `$outCountryCode` VARCHAR(2)
)
BEGIN
	SET $outResultCode = 0;
	SELECT clanId INTO @isExistClan FROM clan_members WHERE userId=$userId;
	IF @isExistClan IS NOT null THEN
		IF @isExistClan <> 0 THEN
			SET $outResultCode = 1;
		END IF;
	END IF;

	SELECT currentSeasonRankPoint INTO @userRankPoint FROM accounts WHERE userId = $userId;

	SELECT clanName, countryCode, memberCount, clanPublicType, joinConditionRankPoint 
	INTO @clanName, @countryCode, @memberCount, @clanPublicType, @joinConditionRankPoint 
	FROM clans WHERE clanId = $clanId FOR UPDATE;

	IF @clanName IS null THEN
		SET $outResultCode = 5;
	ELSE
		IF @memberCount >= $fullMemberCount THEN
			SET $outResultCode = 2;
		ELSE
			IF $isConditionCheck = 1 THEN
				IF @clanPublicType = 2 THEN
					SET $outResultCode = 3;
				ELSE
					IF @joinConditionRankPoint > @userRankPoint THEN
						SET $outResultCode = 4;
					END IF;
				END IF;
			END IF;
		END IF;
	END IF;

	IF $outResultCode = 0 THEN
		UPDATE clans SET rankPoint = rankPoint + @userRankPoint, memberCount = memberCount + 1 WHERE clanId = $clanId;
		UPDATE accounts SET clanName = @clanName WHERE userId = $userId;
		INSERT INTO clan_members (clanId, userId, memberGrade) VALUES ($clanId, $userId, 4)
		ON DUPLICATE KEY UPDATE clanId = $clanId, memberGrade = 4;

		SET $outUserRankPoint = @userRankPoint;
		SET $outCountryCode = @countryCode;
	END IF;
END|
DELIMITER ;