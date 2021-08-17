ALTER TABLE `account`
	CHANGE COLUMN `password` `password` VARCHAR(50) NOT NULL AFTER `username`;

Drop Procedure IF EXISTS `p_insertNewuser`;

DELIMITER $$
CREATE PROCEDURE `p_insertNewuser`(
	IN  `$username` CHAR(20),
	IN  `$password` CHAR(15),
	IN  `$setRunmode` CHAR(15)
)
	
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	START TRANSACTION;
		INSERT INTO account(username, password, lastLoginDate, setRunmode) VALUES($username, PASSWORD($password), now(), $setRunmode);
	COMMIT;
END$$
DELIMITER ;




Drop Procedure IF EXISTS `p_updateUserPW`;

DELIMITER ##
CREATE PROCEDURE `p_updateUserPW`(
	IN  `$username` CHAR(20),
	IN  `$password` CHAR(15)
)
	
BEGIN
  	DECLARE EXIT HANDLER FOR SQLEXCEPTION
  	BEGIN
  		ROLLBACK;
  		
  		GET DIAGNOSTICS CONDITION 1 @sqlstate = RETURNED_SQLSTATE, @errno = MYSQL_ERRNO, @text = MESSAGE_TEXT; 
  		
		SET @text = CONCAT(@sqlstate, "|", @text);
		SIGNAL SQLSTATE '02000' SET MYSQL_ERRNO = @errno, MESSAGE_TEXT = @text;
  	END;

	START TRANSACTION;

		UPDATE account SET password = PASSWORD($password) WHERE username = $username;
	COMMIT;
END##
DELIMITER ;
