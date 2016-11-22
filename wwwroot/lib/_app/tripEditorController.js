!function(){"use strict";function t(t,n){var s=this;s.tripName=t.tripName,s.stops=[],s.errorMessage="",s.isBusy=!0,s.newStop={};var a="/api/trips/"+s.tripName+"/stops",e=function(t){angular.copy(t.data,s.stops),o(s.stops)},i=function(){s.errorMessage="Failed to load stops"},r=function(){s.isBusy=!1};n.get(a).then(e,i).finally(r),s.addStop=function(){s.isBusy=!0;var t=function(t){s.stops.push(t.data),o(s.stops),s.newStop={}},e=function(){s.errorMessage="Failed to add new stop"},i=function(){s.isBusy=!1};n.post(a,s.newStop).then(t,e).finally(i)}}function o(t){if(t&&t.length>0){var o=_.map(t,function(t){return{lat:t.latitude,long:t.longitude,info:t.name}});travelMap.createMap({stops:o,selector:"#map",currentStop:0,initialZoom:3})}}angular.module("app-trips").controller("tripEditorController",t),t.$inject=["$routeParams","$http"]}();