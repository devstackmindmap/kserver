DROP PROCEDURE IF EXISTS `p_outClan`;
DELIMITER |
CREATE PROCEDURE `p_outClan`(
	IN `$userId` INT,
	IN `$targetId` INT,
	IN `$isBanish` INT,
	OUT `$outResultCode` INT,
	OUT `$outUserRankPoint` INT,
	OUT `$outCountryCode` VARCHAR(2),
	OUT `$outClanId` INT
)
BEGIN
	SET $outResultCode = 0;
	SELECT memberGrade, clanId INTO @targetMemberGrade, @targetClanId FROM clan_members WHERE userId=$targetId;

	IF $isBanish = 1 THEN
		SELECT memberGrade, clanId INTO @userMemberGrade, @userClanId FROM clan_members WHERE userId=$userId;

		IF @userMemberGrade = null OR @targetMemberGrade = null THEN
			SET $outResultCode = 1;
		END IF;

		IF @userClanId <> @targetClanId THEN
			SET $outResultCode = 1;
		END IF;

		IF @userMemberGrade >= 3 THEN
			SET $outResultCode = 1;
		END IF;

		IF @userMemberGrade = 2 AND @targetMemberGrade <= 2 THEN
			SET $outResultCode = 1;
		END IF;
	END IF;

	IF $outResultCode = 0 THEN
		SELECT currentSeasonRankPoint INTO @targetUserRankPoint FROM accounts WHERE userId = $targetId;
		UPDATE clans SET rankPoint = rankPoint - @targetUserRankPoint, memberCount = memberCount - 1 WHERE clanId = @targetClanId;
		UPDATE accounts SET clanName = '' WHERE userId = $targetId;
		UPDATE clan_members SET clanId = 0, memberGrade = 0 WHERE userId = $targetId;

		SELECT countryCode INTO @clanCountryCode FROM clans WHERE clanId = @targetClanId;

		SET $outUserRankPoint = @targetUserRankPoint;
		SET $outClanId = @targetClanId;
		SET $outCountryCode = @clanCountryCode;
	END IF;
END|
DELIMITER ;