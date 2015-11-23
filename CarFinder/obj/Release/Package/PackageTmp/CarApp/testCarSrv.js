(function () {
    angular.module('car-finder')
        .factory('testCarSrv', ['$http','$q', function ($http,$q) {
            var service = {};

            service.getYears = function () {
                //var d = $q.defer();
                //d.resolve(['2000', '2001', '2002', '2003']);
                //return d.promise;

                return $http.post('/api/WebApi/AllYears').then(function (response) {
                    return response.data;
                });
            }

            service.getMakes = function (selected) {
                
                return $http.post('/api/WebApi/MakesByYear', selected).then(function (response) {
                    return response.data;
                });

            }

            service.getModels = function (selected) {

                return $http.post('/api/WebApi/ModelsByYearMake', selected).then(function (response) {
                    return response.data;
                });
                
            }

            service.getTrims = function (selected) {
                return $http.post('/api/WebApi/Trims', selected).then(function (response) {
                    return response.data;
                });


            }


            service.GetCar = function (id) {
                // Convert id to object
                var Id = { id: id };

                return $http.post('/api/WebApi/GetCar',Id).then(function (response){
                    return response.data;
                });
            
            }

            service.GetSearchCar = function (selected) {
                
                return $http.post('/api/WebApi/GetSearchCar', selected).then(function (response) {
                    return response.data;
                });
                
            }

            service.GetCarCount = function (selected) {

                return $http.post('/api/WebApi/GetCarCount', selected).then(function (response) {
                    return response.data;
                });

            }



            return service;
        }]);
})();