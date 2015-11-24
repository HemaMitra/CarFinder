(function () {
    angular.module('car-finder')
    .controller('CarController', ['$q', 'testCarSrv', '$modal', function ($q, testCarSrv, $modal) {
        var self = this;

        this.selected =
            {
                year: '2015',
                make: '',
                model: '',
                trim: '',
                filter: '',
                paging: 'true',
                page: '0',
                perpage: '15',
                sortcolumn: 'year',
                sortdirection: '',
                sortByReverse: 'true',
                id:''
            }
        this.options =
            {
                year: '',
                make: '',
                model: '',
                trim: ''
            }

        this.cars = [];
        this.carDetails = [];
        this.count = 0;
        this.loading = false;

        this.getYears = function () {

            testCarSrv.getYears().then(function (data) {
                self.options.years = data;
                //self.GetSearchCar();
            })
        }

        this.getMakes = function () {

            self.selected.make = '',
            self.options.makes = '',
            self.selected.model = '',
            self.options.models = '',
            self.selected.trim = '',
            self.options.trims = '',
            self.cars = [];

            testCarSrv.getMakes(self.selected).then(function (data) {
                self.options.makes = data;
            });
            self.GetSearchCar(self.selected);
            //self.GetCarCount(self.selected);
            //self.LoadCars();
        }

        this.getModels = function () {

            self.selected.model = '',
            self.options.models = '',
            self.selected.trim = '',
            self.options.trims = '',
            self.cars = [];

            testCarSrv.getModels(self.selected).then(function (data) {
                self.options.models = data;
            });
            self.GetSearchCar(self.selected);
            //self.GetCarCount(self.selected);
            //self.LoadCars();
        }

        this.getTrims = function () {

            self.selected.trim = '',
            self.options.trims = '',
            self.cars = [];

            testCarSrv.getTrims(self.selected).then(function (data) {
                self.options.trims = data;
            });
            self.GetSearchCar(self.selected);
            //self.GetCarCount();
            //self.LoadCars();
        }

        this.getCar = function () {

            self.carDetails = [];
            testCarSrv.getCar(self.selected).then(function (data) {
                self.carDetails = data;
            });
        }

        this.GetCarCount = function () {
            testCarSrv.GetCarCount(self.selected)
                .then(function (data) {
                    self.count = data;
                });
        }

        this.GetSearchCar = function () {
            if (!self.loading) {
                self.loading = true;
                // created a copy for trNgGrid because start page no is 0.
                var cpy = angular.copy(self.selected);
                cpy.page += 1;

                $q.all([testCarSrv.GetSearchCar(cpy), testCarSrv.GetCarCount(cpy)])
                            .then(function (data) {
                                self.cars = data[0];
                                self.count = data[1];
                                self.loading = false;
                            });
            }
        }




        //this.LoadCars = function () {
        //    this.GetCarCount();
        //    this.GetSearchCar();
        //}

        this.getYears();

        this.open = function (id) {
            //console.log(id);
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'carModal.html',
                controller: 'carModalController as cm',
                size: 'lg',
                resolve: {
                    car: function () {
                        return testCarSrv.GetCar(id);
                    }
                }

            });
        }

    }])


    // Angular Modal Controller

    angular.module('car-finder').controller('carModalController', function ($modalInstance, car) {

        var self = this;
        self.car = car;

        self.ok = function () {
            $modalInstance.close();
        };

        self.cancel = function () {
            $modalInstance.dismiss();
        }
    });

    

})();
    