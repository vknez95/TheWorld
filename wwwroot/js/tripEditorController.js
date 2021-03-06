(function() {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    tripEditorController.$inject = ['$routeParams', '$http'];

    function tripEditorController($routeParams, $http) {
        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;
        vm.newStop = {};

        var url = "/api/trips/" + vm.tripName + "/stops/";

        var getSuccess = function(response) {
            angular.copy(response.data, vm.stops);
            _showMap(vm.stops);
        };

        var getFail = function() {
            vm.errorMessage = "Failed to load stops";
        };

        var getFinally = function() {
            vm.isBusy = false;
        };

        $http.get(url).then(getSuccess, getFail).finally(getFinally);

        vm.addStop = function() {
            vm.isBusy = true;

            var postSuccess = function(response) {
                vm.stops.push(response.data);
                _showMap(vm.stops);
                vm.newStop = {};
            };

            var postFail = function() {
                vm.errorMessage = "Failed to add new stop";
            };

            var postFinally = function() {
                vm.isBusy = false;
            };

            $http.post(url, vm.newStop).then(postSuccess, postFail).finally(postFinally);
        };

        vm.deleteStop = function(stopId) {
            vm.isBusy = true;

            var deleteSuccess = function() {
                $http.get(url).then(getSuccess, getFail).finally(getFinally);
            };

            var deleteFail = function(err) {
                vm.errorMessage = err;
            };

            var deleteFinally = function() {
                vm.isBusy = false;
            };

            $http.delete(url + stopId).then(deleteSuccess, deleteFail).finally(deleteFinally);
        };
    }

    function _showMap(stops) {
        if (stops && stops.length > 0) {
            var mapStops = _.map(stops, function(item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                };
            });;

            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 0,
                initialZoom: 3
            });
        }
    }
})();