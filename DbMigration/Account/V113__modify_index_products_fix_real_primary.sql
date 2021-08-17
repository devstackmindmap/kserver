ALTER TABLE `_products_fix_real`
	DROP PRIMARY KEY,
	ADD PRIMARY KEY (`productId`, `platform`);