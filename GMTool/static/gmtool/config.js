var krApp = angular.module('krApp', []);

krApp.config(function ($interpolateProvider) {
    $interpolateProvider.startSymbol('[[').endSymbol(']]');
});