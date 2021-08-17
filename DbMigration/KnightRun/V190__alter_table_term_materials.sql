ALTER TABLE `term_materials`
	ADD COLUMN `termMaterialType` INT(11) NOT NULL AFTER `userId`,
	DROP INDEX `UX_users_userId`,
	ADD UNIQUE INDEX `UX_term_materials_userId_termMaterialType` (`userId`, `termMaterialType`);