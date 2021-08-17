ALTER TABLE `units`
	CHANGE COLUMN `unitId` `id` INT(11) UNSIGNED NOT NULL AFTER `userId`;

ALTER TABLE `units`
	DROP INDEX `UX_units_userId_unitId`,
	ADD UNIQUE INDEX `UX_units_userId_id` (`userId`, `id`);

RENAME TABLE `cards` TO `skills`;

ALTER TABLE `skills`
	CHANGE COLUMN `cardId` `id` INT(11) UNSIGNED NOT NULL AFTER `userId`;

ALTER TABLE `skills`
	DROP INDEX `UX_cards_userId_cardId`,
	ADD UNIQUE INDEX `UX_skill_userId_id` (`userId`, `id`);

CREATE TABLE `weapons` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`level` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`count` INT(11) NOT NULL DEFAULT '0',
	`unitId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_weapons_userId_id` (`userId`, `id`),
	INDEX `IX_weapons_unitId` (`unitId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `armors` (
	`seq` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
	`userId` INT(11) UNSIGNED NOT NULL,
	`id` INT(11) UNSIGNED NOT NULL,
	`level` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	`count` INT(11) NOT NULL DEFAULT '0',
	`unitId` INT(11) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_armors_userId_id` (`userId`, `id`),
	INDEX `IX_armors_unitId` (`unitId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

DROP TABLE equipments;