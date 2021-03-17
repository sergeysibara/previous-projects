(function () {
    angular.module('mindMapApp', [
        'ngRoute',
        'mindMapApp.about',
        'mindMapApp.mindmap'
    ]).config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/about', {
                templateUrl: 'views/about/about.html',
                controller: 'AboutCtrl',
                title: 'About'
            })
            .when('/mindmap', {
                templateUrl: 'views/mindmap/mindmap.html',
                controller: 'MindMapCtrl',
                title: 'MindMap'
            });
        // .otherwise({redirectTo: '/about'});
    }]).run(['$rootScope',
         ($rootScope) => {
            $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
                $rootScope.title = current.$$route !== undefined ? current.$$route.title : 'Application';
            });
        }]);
}());