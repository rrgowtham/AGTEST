angular
.module("UserManager")
.controller("tableauController", ["$scope", "userRestFactory", "$window", function ($scope, userRestFactory, win) {

    $scope.$on('IdleTimeout', function () {
        win.location.href = "/Account/Logout";
    });

    //clear out filter text
    $scope.filter = {
        internalUsersFilterText: "",
        externalUsersFilterText: ""
    };

    var fnError = function (rsp) {
        $scope.ServerResponse = { Errors: ["Could not process your request"], Sucess: false };
    },    
    fnGetTableauViewsSuccess = function (rsp) {
        if (rsp.data && !rsp.data.Success) {
            $scope.ServerResponse = rsp.data;
        }
        $scope.getTableauViewsResponse = rsp.data;
        $scope.resetForm();
    };

    $scope.setExpandState = function (view) {

        view.IsOpen = !view.IsOpen;

        $scope.currentExpandedItem = view;

        //clear out filter text
        $scope.filter = {
            internalUsersFilterText: "",
            externalUsersFilterText:""
        };

        //if expanding then load users
        if (view.IsOpen) {
            //pull users for current view and display
            userRestFactory.getUsersForView($scope.currentExpandedItem.ViewId).then(function (xhrResponse) {
                //success                
                if (xhrResponse.data.Success && xhrResponse.data.Data) {
                    $scope.usersList = xhrResponse.data.Data;                    
                } else {
                    $scope.ServerResponse = xhrResponse.data;
                }                
            }, fnError);
        }

        if ($scope.previousExpandedItem == null || $scope.previousExpandedItem == undefined) {
            $scope.previousExpandedItem = view;
            return;
        }

        if ($scope.currentExpandedItem !== $scope.previousExpandedItem) {
            $scope.previousExpandedItem.IsOpen = false;
        }
        $scope.previousExpandedItem = $scope.currentExpandedItem;              
    };

    //filter for external users
    $scope.fnExternalUserFilter = function (txt) {
        console.log(txt);
        return function (user) {
            if (user.UserType == "EXTERNAL" && user.Username.indexOf(txt) !== -1) {
                return user;
            }
        };
    };

    //filter for internal users
    $scope.fnInternalUserFilter = function (txt) {
        console.log(txt);
        return function (user) {
            if (user.UserType == "INTERNAL" && user.Username.indexOf(txt) !== -1) {
                return user;
            }
        };
    };

    $scope.updateUserAssociation = function () {        
        userRestFactory.updateTableauUserInfo($scope.currentExpandedItem.ViewId, $scope.usersList).then(function (xhrResponse) {
            $scope.ServerResponse = xhrResponse.data;
            //success
            if (xhrResponse.data.Success && xhrResponse.data.Data) {
                //close the expanded state
                $scope.cancelOut();
            }
        }, fnError);
    };

    $scope.cancelOut = function () {
        //collapse the current expanded panel and clear out users list
        $scope.resetExpandedState();
        $scope.usersList = null;
    };

    $scope.resetExpandedState = function () {
        if ($scope.previousExpandedItem != null || $scope.previousExpandedItem != undefined) {
            $scope.previousExpandedItem.IsOpen = false;
        }
        if ($scope.currentExpandedItem != null || $scope.currentExpandedItem != undefined) {
            $scope.currentExpandedItem.IsOpen = false;
        }
    };

    $scope.resetForm = function () {
        $scope.tabView = {};
    };

    //tableau server url load
    userRestFactory.getTableauUrl().then(function (rsp) {
        $scope.tableauServerUrl = rsp.data;
    });

    //show add form
    $scope.inEditMode = false;

    //get all tableau views
    userRestFactory.getAllTableauViews().then(fnGetTableauViewsSuccess, fnError);

    //for cancelling out of update form
    $scope.revertUpdateForm = function () {
        //clear any form fields
        $scope.tabView = {};

        //revert to add mode
        $scope.inEditMode = false;
    };

    $scope.closeErrorBox = function () {
        $scope.ServerResponse = null;
    };    

    //For updating add form to update
    $scope.populateModifyForm = function (tabInfo) {
        $scope.tabView = angular.copy(tabInfo);
        $scope.inEditMode = true;
    };

    $scope.addTableauDetail = function () {
        userRestFactory.addTableauViewInfo($scope.tabView).then(function (rsp) {
            //success
            $scope.ServerResponse = rsp.data;
            if (rsp.data.Success && rsp.data.Data) {
                //refresh the grid
                userRestFactory.getAllTableauViews().then(fnGetTableauViewsSuccess, fnError);
            }
        }, fnError);
    };

    $scope.updateTableauDetail = function () {
        userRestFactory.updateTableauViewInfo($scope.tabView).then(function (rsp) {

            //success
            $scope.ServerResponse = rsp.data;
            if (rsp.data.Success && rsp.data.Data) {
                //close update form
                $scope.revertUpdateForm();

                //refresh the grid
                userRestFactory.getAllTableauViews().then(fnGetTableauViewsSuccess, fnError);
            }           
        }, fnError);
    };

}]);