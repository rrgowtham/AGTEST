angular
.module("UserManager")
.controller("internalusersController", ["$scope", "userRestFactory", "$window", function ($scope, userRestFactory, win) {

    $scope.$on('IdleTimeout', function () {
        win.location.href = "/Account/Logout";
    });

    var fnError = function (rsp) {
            $scope.ServerResponse = { Errors: ["Could not process your request"], Sucess: false };
    },
    fnGetUsersSuccess = function (rsp) {        
        if (rsp.data && !rsp.data.Success) {
            $scope.ServerResponse = rsp.data;
        }
        $scope.getUsersResponse = rsp.data;
    };

    //show add form
    $scope.inEditMode = false;

    //Get all user information
    userRestFactory.getAllInternalUsers().then(fnGetUsersSuccess, fnError);

    //for cancelling out of update form
    $scope.revertUpdateForm = function () {
        //clear any form fields
        $scope.newUser = {};

        //revert to add mode
        $scope.inEditMode = false;
    };

    $scope.closeErrorBox = function () {
        $scope.ServerResponse = null;
    };

    $scope.updateUserDetail = function () {
        userRestFactory.updateInternalUser($scope.newUser).then(function (rsp) {
            //success
            $scope.ServerResponse = rsp.data;
            if (rsp.data.Success && rsp.data.Data) {
                //close update form
                $scope.revertUpdateForm();

                //refresh the grid
                userRestFactory.getAllInternalUsers().then(fnGetUsersSuccess, fnError);
            }
        }, fnError);
    };

    //For updating add form to update
    $scope.populateModifyForm = function (user) {
        $scope.newUser = angular.copy(user);
        $scope.inEditMode = true;
    };

    $scope.addUserDetail = function () {
        userRestFactory.mapInternalUser($scope.newUser).then(function (rsp) {
            //success
            $scope.ServerResponse = rsp.data;            
            if (rsp.data.Success && rsp.data.Data) {
                //refresh the grid
                userRestFactory.getAllInternalUsers().then(fnGetUsersSuccess, fnError);
                $scope.resetForm();
            }
        }, fnError);
    };

    $scope.resetForm = function () {
        $scope.newUser = {};
    };

}]);