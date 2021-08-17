INSERT INTO `_schedules` (`scheduleId`, `scheduleType`, `startDateTime`, `endDateTime`) VALUES (1, 1, '2019-06-19 00:00:00', '2019-06-21 23:59:59');
INSERT INTO `_items` (`seq`, `itemId`, `itemType`, `id`, `value`) VALUES (1, 1, 10, 1001, 0);
INSERT INTO `_items` (`seq`, `itemId`, `itemType`, `id`, `value`) VALUES (2, 1, 5, 0, 2000);
INSERT INTO `_store` (`storeId`, `storeType`, `goodsImage`, `text`, `purchaseMaterialType`, `cost`, `itemId`, `scheduleId`) VALUES (1, 1, 'store_item_box1', '이벤트 상품', 2, 200, 1, 1);