ALTER TABLE `_products_event_real`
	DROP PRIMARY KEY,
	ADD PRIMARY KEY (`productId`, `platform`);