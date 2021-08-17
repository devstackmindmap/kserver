DROP PROCEDURE  IF EXISTS `p_getProducts`;

DELIMITER |
CREATE PROCEDURE `p_getProducts`(
	IN `$utcNow` VARCHAR(19),
	IN `$utcNowAddHour` VARCHAR(19),
	IN `$currency` VARCHAR(2),
	IN `$languageType` VARCHAR(2)
)
BEGIN
	SELECT a.productId, startDateTime, endDateTime, storeType, productBannerType, productType, 
	materialType, saleCost, cost, countOfPurchases, ifnull(b.productText, '') AS productText, rewardId
	FROM _products_event_digital a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType
	WHERE endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;
	
	SELECT a.productId, startDateTime, endDateTime, storeProductId, storeType, productBannerType, productType, 
	b.saleCost, b.cost, countOfPurchases, ifnull(c.productText, '') AS productText, rewardId
	FROM _products_event_real a
	LEFT OUTER JOIN _products_real_cost b ON b.productId = a.productId AND b.currencyType = $currency
	LEFT OUTER JOIN _products_text c ON c.productId = a.productId AND c.languageType = $languageType
	WHERE endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;

	SELECT a.productId, startDateTime, endDateTime, storeType, productBannerType, productType, materialType, 
	saleCost, cost, ifnull(b.productText, '') AS productText, rewardId
	FROM _products_fix_digital a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType;

	SELECT a.productId, startDateTime, endDateTime, storeProductId, storeType, productBannerType, productType, 
	b.saleCost, b.cost, ifnull(c.productText, '') AS productText, rewardId
	FROM _products_fix_real a
	LEFT OUTER JOIN _products_real_cost b ON b.productId = a.productId AND b.currencyType = $currency
	LEFT OUTER JOIN _products_text c ON c.productId = a.productId AND c.languageType = $languageType;
END|
DELIMITER ;