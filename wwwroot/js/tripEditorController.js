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

        var url = "/api/trips/" + vm.tripName + "/stops";

        var getSuccess = function(response) {
            angular.copy(response.data, vm.stops);
            _showMap(vm.stops);
        };

        var getFail = function(err) {
            vm.errorMessage = "Failed to load stops: " + err;
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
                currentStop: 1,
                initialZoom: 3
            });
        }
    }
})();