CREATE TABLE `product_purchases` (
	`seq` INT(11) NOT NULL AUTO_INCREMENT,
	`userId` INT(10) UNSIGNED NOT NULL,
	`productId` INT(10) UNSIGNED NOT NULL,
	`countOfPurchases` INT(10) NOT NULL,
	PRIMARY KEY (`seq`),
	UNIQUE INDEX `UX_product_purchases_userId_productId` (`userId`, `productId`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
