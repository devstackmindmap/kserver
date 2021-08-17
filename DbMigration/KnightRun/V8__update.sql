
ALTER TABLE `decks` DROP INDEX `UX_decks_userId_modeType_deckNum_slotType_orderNum`;

ALTER TABLE `cards`
  MODIFY COLUMN `count` int(11) NOT NULL DEFAULT '0';

ALTER TABLE `decks` ADD UNIQUE KEY `UX_decks_userId_modeType_deckNum_slotType_orderNum`(`userId`,`modeType`,`deckNum`,`slotType`,`orderNum`);

ALTER TABLE `equipments`
  MODIFY COLUMN `count` int(11) NOT NULL DEFAULT '0';

CREATE TABLE `rewards` (
  `seq` int(11) unsigned NOT NULL auto_increment,
  `rewardUId` int(11) unsigned NOT NULL,
  `userId` int(11) unsigned NOT NULL,
  `rewardType` tinyint(3) unsigned NOT NULL,
  `classId` int(11) unsigned NOT NULL,
  `rewardOpenTimeTick` bigint(20) NOT NULL,
  PRIMARY KEY (`seq`),
  UNIQUE KEY `UX_rewards_rewardUId_userId_rewardType`(`rewardUId`,`userId`,`rewardType`)
) ENGINE=InnoDB;

ALTER TABLE `units`
  MODIFY COLUMN `count` int(11) NOT NULL DEFAULT '0';

DROP PROCEDURE `p_getLoginInfo`;

DELIMITER $$
CREATE PROCEDURE `p_getLoginInfo`(
	IN `$userId` INT

)
BEGIN
	DECLARE EXIT HANDLER FOR SQLEXCEPTION      
	BEGIN
	  ROLLBACK;
	  
	  GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
	  
	SET @text = CONCAT(@sqlstate, "|", @text);
	SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
	END;

    SELECT gold FROM users WHERE userId = $userId;
    SELECT cardId, level, count FROM cards WHERE userId = $userId;
    SELECT unitId, level, count FROM units WHERE userId = $userId;
    SELECT rankType, rankPoint, rankTierMatchingId FROM ranks WHERE userId = $userId;
    SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = $userId;
    SELECT equipId, level, count, unitId FROM equipments WHERE userId = $userId;
    SELECT rewardUId, rewardType, classId, rewardOpenTimeTick FROM rewards WHERE userId = $userId;
END$$
DELIMITER ;
