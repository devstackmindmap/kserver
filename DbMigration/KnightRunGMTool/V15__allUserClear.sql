DROP PROCEDURE  IF EXISTS `p_truncateAllUsers`;

DELIMITER |
CREATE PROCEDURE `p_truncateAllUsers`(
	IN `$YN` char
	
)
BEGIN
	IF $YN = 'Y' THEN 
		truncate table users;
		truncate table units;
		truncate table cards;
		truncate table armors;
		truncate table weapons;
		truncate table stage_levels;
		truncate table decks;
		truncate table infusion_boxes;
		truncate table skins;
		truncate table square_object;
		truncate table ranks;
		truncate table corrections;
		truncate table friends;
		truncate table request_friends;
		truncate table quests;
		truncate table emoticons;
		truncate table battle_records;
		truncate table battle_record_behaviors;
		truncate table accounts_social;
	END IF;
	
END|
DELIMITER ;