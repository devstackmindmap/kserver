DROP PROCEDURE IF EXISTS `p_outClan`;
DELIMITER |
CREATE PROCEDURE `p_outClan`(
	IN `$userId` INT,
	IN `$targetId` INT,
	IN `$isBanish` INT,
	OUT `$outResultCode` INT,
	OUT `$outUserRankPoint` INT,
	OUT `$outCountryCode` VARCHAR(2),
	OUT `$outClanId` INT,
	OUT `$outClanDelete` INT
)
BEGIN
	SET $outClanDelete = 0;
	SET $outResultCode = 0;
	SELECT memberGrade, clanId INTO @targetMemberGrade, @targetClanId FROM clan_members WHERE userId=$targetId;
	IF @targetMemberGrade = null THEN
		SET $outResultCode = 2;
	ELSE
		IF $isBanish = 1 THEN
			SELECT memberGrade, clanId INTO @userMemberGrade, @userClanId FROM clan_members WHERE userId=$userId;

			IF (@userMemberGrade = null) OR
				(@userClanId <> @targetClanId) OR
				(@userMemberGrade >= 3) OR
				(@targetMemberGrade <> 4)
			THEN
				SET $outResultCode = 1;
			END IF;
		END IF;

		SET @isClanDelete = 0;
		IF @targetMemberGrade = 1 THEN
			SELECT memberCount INTO @memberCount FROM clans WHERE clanId = @targetClanId FOR UPDATE;
			IF @memberCount = 1 THEN
				SET @isClanDelete = 1;
			ELSE
				SET $outResultCode = 1;
			END IF;
		END IF;

		IF $outResultCode = 0 THEN
			SELECT countryCode INTO @clanCountryCode FROM clans WHERE clanId = @targetClanId;
			SELECT currentSeasonRankPoint INTO @targetUserRankPoint FROM accounts WHERE userId = $targetId;
			UPDATE accounts SET clanName = '' WHERE userId = $targetId;
			UPDATE clan_members SET clanId = 0, memberGrade = 0 WHERE userId = $targetId;

			IF @isClanDelete = 0 THEN
				UPDATE clans SET rankPoint = rankPoint - @targetUserRankPoint, memberCount = memberCount - 1 WHERE clanId = @targetClanId;
			ELSE
				SET $outClanDelete = 1;
				DELETE FROM clans WHERE clanId = @targetClanId;
			END IF;

			SET $outUserRankPoint = @targetUserRankPoint;
			SET $outClanId = @targetClanId;
			SET $outCountryCode = @clanCountryCode;
		END IF;
	END IF;
END|
DELIMITER ;