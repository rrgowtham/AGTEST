angular
    .module("UserManager")
    .controller("usersController", ["$scope", "userRestFactory","$window", function ($scope, userRestFactory,win) {
        //initialize
        $scope.getUsersResponse = {
            Success: true
        };

        var fnGetUsrlistCallback = function (xhr) {
            //user list table rendered with this scope variable            
            $scope.getUsersResponse = xhr.data;
        };     

        $scope.$on('IdleTimeout', function () {
            win.location.href = "/Account/Logout";
        });

        //Get All users and list them in Grid
        userRestFactory.getAllUsers().then(fnGetUsrlistCallback, fnGetUsrlistCallback);       

    }])