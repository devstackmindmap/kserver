DROP PROCEDURE  IF EXISTS `p_getProducts`;

DELIMITER |
CREATE PROCEDURE `p_getProducts`(
	IN `$platform` INT,
	IN `$utcNow` VARCHAR(19),
	IN `$utcNowAddHour` VARCHAR(19),
	IN `$languageType` VARCHAR(2)
)
BEGIN
	SELECT a.productId, startDateTime, endDateTime, storeType, productType, 
	materialType, saleCost, cost, countOfPurchases, ifnull(b.productText, '') AS productText
	FROM _products_event_digital a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType
	WHERE endDateTime > $utcNow AND startDateTime <= $utcNowAddHour;
	
	SELECT a.productId, startDateTime, endDateTime, storeProductId, storeType, productType, 
	saleCost, cost, countOfPurchases, ifnull(b.productText, '') AS productText
	FROM _products_event_real a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType
	WHERE endDateTime > $utcNow AND startDateTime <= $utcNowAddHour AND platform = $platform;

	SELECT a.productId, storeType, productType, materialType, 
	cost, ifnull(b.productText, '') AS productText
	FROM _products_fix_digital a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType;

	SELECT a.productId, storeProductId, storeType, productType, 
	cost, ifnull(b.productText, '') AS productText
	FROM _products_fix_real a
	LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = $languageType
	WHERE platform = $platform ORDER BY cost ASC;
END|
DELIMITER ;