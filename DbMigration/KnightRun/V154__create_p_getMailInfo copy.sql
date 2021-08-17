DROP PROCEDURE  IF EXISTS `p_getMailInfo`;

DELIMITER |
CREATE PROCEDURE `p_getMailInfo`(
	IN `$userId` INT,
	IN `$utcNow` VARCHAR(19),
	IN `$utcNowAddHour` VARCHAR(19)
)
BEGIN
	SELECT MAX(mailId) as maxPublicMailId FROM user_mail_public WHERE userId = $userId GROUP BY userId;

	SELECT mailId, startDateTime, endDateTime, isRead FROM user_mail_public 
	WHERE userId = $userId AND isDeleted = 0 and endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;

	SELECT mailId, startDateTime, endDateTime, isRead, mailIcon, productId, mailTitle, mailText FROM user_mail_private 
	WHERE userId = $userId AND isDeleted = 0 and endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;

	SELECT mailId, startDateTime, endDateTime, isRead FROM user_mail_system 
	WHERE userId = $userId AND isDeleted = 0 and endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;
END|
DELIMITER ;