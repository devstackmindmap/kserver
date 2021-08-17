RENAME TABLE `skills` TO `cards`;

ALTER TABLE `cards`
	DROP INDEX `UX_skill_userId_id`,
	ADD UNIQUE INDEX `UX_cards_userId_id` (`userId`, `id`);