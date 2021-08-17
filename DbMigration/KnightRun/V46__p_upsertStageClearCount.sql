DROP PROCEDURE IF EXISTS `p_upsertStageClearCount`;

DELIMITER |
CREATE PROCEDURE `p_upsertStageClearCount`(
	IN `$userId` INT,
  IN `$stageLevelId` INT

)
BEGIN
	INSERT INTO stage_levels (userId, stageLevelId, clearCount) VALUES ($userId, $stageLevelId, 1)
	ON DUPLICATE KEY UPDATE clearCount = clearCount + 1;
	SELECT clearCount FROM stage_levels WHERE userId = $userId AND stageLevelId=$stageLevelId;
END|
DELIMITER ;