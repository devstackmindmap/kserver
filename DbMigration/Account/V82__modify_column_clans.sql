ALTER TABLE `clans`
	CHANGE COLUMN `clanSymbolId` `clanSymbolId` INT(11) UNSIGNED NULL DEFAULT '0' AFTER `clanName`;
    
ALTER TABLE `clans`
	CHANGE COLUMN `clanName` `clanName` VARCHAR(50) NOT NULL DEFAULT '' AFTER `clanMasterUserId`;

UPDATE clans SET clanSymbolId = 0 WHERE clanSymbolId IS NULL;
UPDATE clans SET clanName = 0 WHERE clanName IS NULL;