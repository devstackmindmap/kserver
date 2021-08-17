DROP PROCEDURE IF EXISTS `p_modifyProfileClan`;

DELIMITER |
CREATE PROCEDURE `p_modifyProfileClan`(
	IN `$userId` INT,
	IN `$clanId` INT,
	IN `$clanSymbolId` INT,
	IN `$clanPublicType` INT,
	IN `$joinConditionRankPoint` INT,
	IN `$countryCode` VARCHAR(2),
	IN `$clanExplain` VARCHAR(128),
	OUT `$outResultCode` INT,
	OUT `$outRankPoint` INT,
	OUT `$oldCountryCode` VARCHAR(2)
)
BEGIN
	SET $outResultCode = 0;
	SELECT clanId, memberGrade INTO @isExistClan, @memberGrade FROM clan_members WHERE userId=$userId;
	IF @isExistClan IS null THEN
		SET $outResultCode = 1;
	END IF;

	IF (@isExistClan <> $clanId) OR
		((@memberGrade <> 1) AND (@memberGrade <> 2))
	THEN
		SET $outResultCode = 1;
	END IF;
	
	IF $outResultCode = 0 THEN
		SELECT rankPoint, countryCode INTO @rankPoint, @countryCode 
		FROM clans WHERE clanId=$clanId FOR UPDATE;

		SET $outRankPoint = @rankPoint;
		SET $oldCountryCode = @countryCode;

		UPDATE clans SET countryCode = $countryCode, clanPublicType = $clanPublicType,
		joinConditionRankPoint = $joinConditionRankPoint, clanSymbolId = $clanSymbolId,
		clanExplain = $clanExplain WHERE clanId = $clanId;
	END IF;
	
END|
DELIMITER ;