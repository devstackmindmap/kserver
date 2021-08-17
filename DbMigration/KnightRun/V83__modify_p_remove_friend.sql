DROP PROCEDURE  IF EXISTS `p_deleteFriend`;

DELIMITER |
CREATE PROCEDURE `p_deleteFriend`(
	IN `$userId` INT,
	IN `$friendId` INT
)
BEGIN
  	DELETE FROM request_friends WHERE userId = $userId AND friendId = $friendId;
  	DELETE FROM recommend_friends WHERE userId = $userId AND friendId = $friendId;
  	DELETE FROM friends WHERE userId = $userId AND friendId = $friendId;

	  DELETE FROM request_friends WHERE userId = $friendId AND friendId = $userId;
  	DELETE FROM recommend_friends WHERE userId = $friendId AND friendId = $userId;
  	DELETE FROM friends WHERE userId = $friendId AND friendId = $userId;
END|
DELIMITER ;