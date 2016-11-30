(function() {
    "use strict";

    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {
        var vm = this;

        vm.trips = [];
        vm.newTrip = {};
        vm.errorMessage = "";
        vm.isBusy = true;

        vm.addTrip = function() {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function(response) {
                    // Success
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function() {
                    // Failure
                    vm.errorMessage = "Failed to save new trip";
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };

        vm.deleteTrip = function(tripId) {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.delete("/api/trips/" + tripId)
                .then(function() {
                    // Success
                    getTrips();
                }, function() {
                    // Failure
                    vm.errorMessage = "Failed to delete trip";
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        };

        var getTrips = function() {
            $http.get("/api/trips")
                .then(function(response) {
                    // Success
                    // vm.trips = response.data;
                    angular.copy(response.data, vm.trips);
                }, function() {
                    // Failure
                    vm.errorMessage = "Failed to load data";
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        }

        getTrips();
    }
})();