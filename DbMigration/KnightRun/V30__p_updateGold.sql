DELIMITER |
CREATE PROCEDURE `p_updateGold`(
	IN `$userId` INT,
	IN `$deltaGold` INT,
	OUT `$out_gold` INT


)
BEGIN
	SELECT @gold := gold FROM users WHERE userId = $userId;
	SET $out_gold = @gold + $deltaGold;
	
	IF $out_gold < 0 THEN
		SET $out_gold = 0;
	END IF;
	
	UPDATE users SET gold = $out_gold WHERE userId = $userId;
END|
DELIMITER ;

