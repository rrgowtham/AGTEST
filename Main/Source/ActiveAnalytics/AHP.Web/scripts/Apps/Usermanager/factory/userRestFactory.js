angular
    .module("UserManager")
    .factory("userRestFactory", ["$http", function ($http) {

        return {
            getAllUsers: function () {
                return $http.post('UserManager/GetUsersList', null);
            },
            resetPassword: function (userInfo) {
                return $http.post('UserManager/ResetPassword', userInfo);
            },
            lockUser: function (userInfo) {
                return $http.post('UserManager/LockUser', userInfo);
            },
            unlockUser: function (userInfo) {
                return $http.post('UserManager/UnlockUser', userInfo);
            },
            updateUserInfo: function (user) {
                return $http.post('UserManager/Updateuser', user);
            },
            createUser: function (user) {
                return $http.post('UserManager/CreateUser', user);
            },
            sendActivationEmail: function (user) {
                return $http.post("UserManager/ActivateEmail", user);
            },
            getUserDetail: function (username) {
                return $http.post("UserManager/Userdetail", { "username": username });
            },
            activateUser: function (userInfo) {
                return $http.post('UserManager/ActivateUser', userInfo);
            },
            deactivateUser: function (userInfo) {
                return $http.post('UserManager/DeactivateUser', userInfo);
            },
            getAllInternalUsers: function () {
                return $http.post("UserManager/GetInternalUsers");
            },
            mapInternalUser: function (user) {
                return $http.post('UserManager/MapInternalUser', user);
            },
            updateInternalUser: function (user) {
                return $http.post('UserManager/UpdateInternalUser', user);
            },
            getAllTableauViews: function () {
                return $http.post('UserManager/ListTableauViews');
            },
            getTableauUrl: function () {
                return $http.post("UserManager/GetTableauUrl");
            },
            addTableauViewInfo: function (tabInfo) {
                return $http.post('UserManager/AddTableauView', tabInfo);
            },
            updateTableauViewInfo: function (tabInfo) {
                return $http.post('UserManager/UpdateableauView', tabInfo);
            },
            getUsersForView: function (viewId) {
                return $http.post('UserManager/GetUsersForView', { "viewId": viewId });
            },
            updateTableauUserInfo: function (viewId, userInfo) {
                return $http.post("UserManager/UpdateTableauUserInfo", {
                    "viewId": viewId,
                    "userInfo":userInfo
                });
            }
        };

    }]);