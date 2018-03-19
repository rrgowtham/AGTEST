angular
    .module("UserManager")
    .controller("userDetailsController", ["$scope", "$routeParams", "userRestFactory","$window", function ($scope, $routeParams, userRestFactory,win) {

    $scope.userDetail = {
        Success: true
    };

    $scope.allowedRoles = ["User", "Admin"];

    $scope.inEditMode = false;

    $scope.activityInProgress = false;

    //send's a welcome email to user
    $scope.activateEmail = function () {
        $scope.activityInProgress = true;
        userRestFactory.sendActivationEmail($scope.userDetail.Data).then(function (xhr) {
            $scope.userDetail.Data.IsEmailActivated = xhr.data.Data;
            $scope.activityInProgress = false;
        });
    };

    $scope.$on('IdleTimeout', function () {
        win.location.href = "/Account/Logout";
    });

    //reset's the password for the user and send's a email
    $scope.resetPassword = function () {

        $scope.activityInProgress = true;
        userRestFactory.resetPassword($scope.userDetail.Data).then(function (xhr) {
            if (xhr.data.Success) {
                $scope.userDetail.Data.IsEmailActivated = true;
                $scope.userDetail.Data.IsLocked = false;
                $scope.userDetail.Data.ChangePasswordOnLogon = true;
                $scope.userDetail.Data.InvalidLogonAttemptCount = 0;
                //$scope.operationSucceeded = true;
                $scope.ServerResponse = {
                    Success: true
                }
            } else {
                //$scope.operationSucceeded = false;
                $scope.ServerResponse = {
                    Success: false,
                    Error: ["Could not process your request. Please try again"]
                }
            }            
            $scope.activityInProgress = false;
        });

    };

    //Unlocks the user account
    $scope.unlockAccount = function () {
        $scope.activityInProgress = true;
        userRestFactory.unlockUser($scope.userDetail.Data).then(function (xhr) {
            if (xhr.data && xhr.data.Success) {
                $scope.userDetail.Data.IsLocked = false;
                $scope.userDetail.Data.InvalidLogonAttemptCount = 0;
                //$scope.operationSucceeded = true;
                $scope.ServerResponse = {
                    Success: true
                }
            } else {
                //$scope.operationSucceeded = false;
                $scope.ServerResponse = {
                    Success: false,
                    Error: ["Could not process your request. Please try again"]
                }
            }
            $scope.activityInProgress = false;
        });
    };

    //Locks the users account
    $scope.lockAccount = function () {
        $scope.activityInProgress = true;
        userRestFactory.lockUser($scope.userDetail.Data).then(function (xhr) {
            if (xhr.data && xhr.data.Success) {
                $scope.userDetail.Data.IsLocked = true;
                //$scope.operationSucceeded = true;
                $scope.ServerResponse = {
                    Success: true
                }
            }else {
                //$scope.operationSucceeded = false;
                $scope.ServerResponse = {
                    Success: false,
                    Error: ["Could not process your request. Please try again"]
                }
            }
            $scope.activityInProgress = false;
        });
    };

    //When clicked opens uses info for editing 
    $scope.openEditMode = function () {
        //$scope.operationSucceeded = null;
        $scope.ServerResponse = null;
        $scope.inEditMode = true;
        $scope.editDetails = angular.copy($scope.userDetail.Data);
    };

    $scope.updateUser = function () {
        //server side takes care of the validation
        userRestFactory.updateUserInfo($scope.editDetails).then(function (xhr) {
            $scope.ServerResponse = xhr.data;
            if (xhr.data.Success) {
                $scope.userDetail.Data = $scope.editDetails = xhr.data.Data;
                $scope.closeEditMode();
            }
            
        });
    };

    $scope.closeErrorBox = function () {
        $scope.ServerResponse = null;
    };

    //activate and deactivate account
    $scope.activateAccount = function () {
        $scope.activityInProgress = true;
        userRestFactory.activateUser($scope.userDetail.Data).then(function (xhr) {
            if (xhr.data && xhr.data.Success) {
                $scope.userDetail.Data.IsActive = true;
                //$scope.operationSucceeded = true;
                $scope.ServerResponse = {
                    Success: true
                }
            } else {
                //$scope.operationSucceeded = false;
                $scope.ServerResponse = {
                    Success: false,
                    Error: ["Could not process your request. Please try again"]
                }
            }
            $scope.activityInProgress = false;
        });
    };

    $scope.deactivateAccount = function () {
        $scope.activityInProgress = true;
        userRestFactory.deactivateUser($scope.userDetail.Data).then(function (xhr) {
            if (xhr.data && xhr.data.Success) {
                $scope.userDetail.Data.IsActive = false;
                //$scope.operationSucceeded = true;
                $scope.ServerResponse = {
                    Success: true
                }
            } else {
                //$scope.operationSucceeded = false;
                $scope.ServerResponse = {
                    Success: false,
                    Error: ["Could not process your request. Please try again"]
                }
            }
            $scope.activityInProgress = false;
        });
    };

    $scope.closeEditMode = function () {
        //just turn off edit flag
        $scope.inEditMode = false;
        //$scope.operationSucceeded = null;
        //$scope.ServerResponse = null;
    };    

    //check if username parameter is provided
    if ($routeParams.username) {
        $scope.activityInProgress = true;
        userRestFactory.getUserDetail($routeParams.username).then(function (xhr) {
            $scope.userDetail = xhr.data;
            $scope.activityInProgress = false;
        });
    } else {
        $scope.ServerResponse = {
            Success: false,
            Error: ["User information could not be found"]
        }
    }

}]);