angular
    .module("UserManager")
    .controller("createUsersController", ["$scope", "userRestFactory", "$window", function ($scope, userRestFactory, win) {

        var fnSuccess = function (rsp) {
            $scope.ServerResponse = rsp.data;
            if (rsp.data && rsp.data.Success && rsp.data.Data) {
                //clear form on success
                $scope.resetForm();
            }
        },
        fnError = function (rsp) {
            $scope.ServerResponse = { Errors: ["Could not process your request"], Sucess: false };
        };

       $scope.$on('IdleTimeout', function () {
            win.location.href = "/Account/Logout";
        });

        $scope.resetForm = function () {
            //reset form fields
            $scope.user = {
                Role: $scope.allowedRoles[0],
                Username: "",
                Firstname: "",
                Lastname: "",
                Email: "",
                SupplierId: ""
            };
        };

        $scope.allowedRoles = ["User", "Admin"];

        //checking duplicate username
        $scope.doesUsernameExist = function (usr) { };

        //checking duplicate email address
        $scope.doesEmailExist = function (usr) { };

        //creating user account
        $scope.createUser = function (user) {
            userRestFactory.createUser(user).then(fnSuccess, fnError);
        };

    }])
.controller("switchViewController", ["$scope", function ($scope) {
    //Yeah its dummy controller for nothing
}]);