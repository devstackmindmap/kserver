var krApp = angular.module('kr_app', []);
krApp.config(function ($interpolateProvider, $httpProvider) {
    $interpolateProvider.startSymbol('[[').endSymbol(']]');
    var authToken = $('meta[name="csrf-token"]').attr('content');
    $httpProvider.defaults.headers.common["X-CSRF-TOKEN"] = authToken;
});

String.prototype.format = function () {
    a = this;
    for (k in arguments) {
        a = a.replace("{" + k + "}", arguments[k])
    }

    return a
}

