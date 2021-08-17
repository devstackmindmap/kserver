DROP PROCEDURE IF EXISTS `p_modifyMemberGrade`;
DELIMITER |
CREATE PROCEDURE `p_modifyMemberGrade`(
	IN `$userId` INT,
	IN `$targetId` INT,
	IN `$memberGrade` INT,
	OUT `$outResultCode` INT
)
BEGIN
	SET $outResultCode = 0;
	SELECT memberGrade, clanId INTO @userMemberGrade, @userClanId FROM clan_members WHERE userId=$userId;
	SELECT memberGrade, clanId INTO @targetMemberGrade, @targetClanId FROM clan_members WHERE userId=$targetId;
		
	IF (@userMemberGrade = null OR @targetMemberGrade = null) OR
		(@userClanId <> @targetClanId) OR
		@userMemberGrade >= 3 OR
		(@userMemberGrade = 2 AND @targetMemberGrade <= 2) OR
		(@userMemberGrade = 2 AND $memberGrade <= 2 )
	THEN
		SET $outResultCode = 1;
	END IF;

	IF $outResultCode = 0 THEN
		UPDATE clan_members SET memberGrade = $memberGrade WHERE userId = $targetId;
		IF $memberGrade = 1 THEN
			UPDATE clan_members SET memberGrade = 2 WHERE userId = $userId;
		END IF;
	END IF;
END|
DELIMITER ;