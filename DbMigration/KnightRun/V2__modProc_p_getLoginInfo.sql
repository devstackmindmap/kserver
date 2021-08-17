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
    SELECT rankType, rankPoint FROM ranks WHERE userId = $userId;
END$$
DELIMITER ;
