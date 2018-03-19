angular
    .module("UserManager", ["ngRoute", "ngResource", "ngSanitize", "ngMessages", "angular-loading-bar", "ngIdle"])
    .config(["$routeProvider", "KeepaliveProvider", "IdleProvider", function ($routeProvider, KeepaliveProvider, IdleProvider) {


        $routeProvider          
            // route for the contact page
            .when('/manageusers', {
                templateUrl: 'Scripts/Apps/Usermanager/views/home.html',
                controller: 'usersController'
            })
            .when('/createusers', {
                templateUrl: 'Scripts/Apps/Usermanager/views/createuser.html',
                controller: 'createUsersController'
            })
            .when('/user/:username', {
                templateUrl: 'Scripts/Apps/Usermanager/views/userdetails.html',
                controller: 'userDetailsController'
            })
            .when('/manage-internal-users', {
                templateUrl: "Scripts/Apps/Usermanager/views/internalusers.html",
                controller:"internalusersController"
            })
            .when("/tableau", {
                templateUrl: "Scripts/Apps/Usermanager/views/tableauviews.html",
                controller:"tableauController"
            })
            .otherwise({
                redirectTo: '/manageusers'
            });

        //Timeout and session
        IdleProvider.idle(3);
        IdleProvider.timeout(1200);
    }])
    .run(["Idle", function (Idle) {
        Idle.watch();
    }]);;