-- Script generated by MySQL Compare 1.0.1.4 on 2018-08-07 오전 10:09:24

ALTER TABLE `ranks`
  ADD COLUMN `rankTierMatchingId` int(10) NOT NULL DEFAULT '0';

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
    SELECT equipId, unitId, level, count FROM equipments WHERE userId = $userId;
    SELECT unitId, level, count FROM units WHERE userId = $userId;
    SELECT rankType, rankPoint, rankTierMatchingId FROM ranks WHERE userId = $userId;
END$$
DELIMITER ;

