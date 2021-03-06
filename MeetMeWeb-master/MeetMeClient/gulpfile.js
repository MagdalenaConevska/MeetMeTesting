/**
*
* Developed as a part of a project founded by Sorsix
*
* @Authors
*  Tomce Delev
*  Dragan Sahpaski
*  Riste Stojanov
*
**/
var gulp = require('gulp');
var concat = require('gulp-concat');
var templateCache = require('gulp-angular-templatecache');
var rev = require('gulp-rev-append');
var eslint = require('gulp-eslint');
var connect = require('gulp-connect');
var fs = require("fs");

var JS_APP = [
  'app/app.js',
  'app/index/indexController.js',
  'app/account/authInterceptorService.js',
  'app/account/accountController.js',
  'app/account/accountService.js',
  'app/account/authInterceptorService.js',
  'app/account/loginState.js',
  'app/account/registerState.js',
  'app/event/eventController.js',
  'app/event/eventService.js',
  'app/event/eventState.js',
  'app/directives/date-time-picker/dateTimePickerDirective.js',
  'app/user/userService.js',
  'app/calendar/calendarController.js',
  'app/calendar/calendarState.js',
  'app/calendar/calendarService.js',
  'app/profile/profileState.js',
  'app/profile/profileController.js',
  'app/home/homeState.js',
  'app/meeting/meetingController.js',
  'app/meeting/meetingState.js',
  'app/meeting/meetingService.js',
  'app/connection-notification/connectionNotificationController.js',
  'app/connection-notification/connectionNotificationState.js',
  'app/meeting-request/meetingRequestController.js',
  'app/meeting-request/meetingRequestState.js'
];

var TEMPLATES_SRC = [
    'app/default-page/**.html'
];

var FONTS_LIB = [
    'bower_components/components-font-awesome/fonts/fontawesome-webfont.woff2',
    'bower_components/components-font-awesome/fonts/fontawesome-webfont.woff',
    'bower_components/components-font-awesome/fonts/fontawesome-webfont.ttf',
    'bower_components/bootstrap/fonts/glyphicons-halflings-regular.ttf',
    'bower_components/bootstrap/fonts/glyphicons-halflings-regular.woff',
    'bower_components/bootstrap/fonts/glyphicons-halflings-regular.woff2'
];

var CSS_LIB = [
    'bower_components/bootstrap/dist/css/bootstrap.css',
    'bower_components/components-font-awesome/css/font-awesome.min.css',
    'bower_components/smalot-bootstrap-datetimepicker/css/bootstrap-datetimepicker.css',
    'bower_components/glyphicons-halflings/css/glyphicons-halflings',
    'bower_components/bootstrap-additions/dist/bootstrap-additions.css',
    'bower_components/fullcalendar/dist/fullcalendar.css',
    'bower_components/angucomplete-alt/angucomplete-alt.css',
    'bower_components/bootstrap-select/dist/css/bootstrap-select.css',
    'bower_components/angular-ui-select/dist/select.css',
    'bower_components/angular-notification-icons/dist/angular-notification-icons.min.css',
    'bower_components/ng-notify/dist/ng-notify.min.css'
];

var CSS_APP = [
    'css/custom.css',
    'css/calendar.css',
    'css/event.css',
    'css/profile.css'
];

var JS_LIB = [
    'bower_components/jquery/dist/jquery.min.js',
    'bower_components/bootstrap/dist/js/bootstrap.min.js',
    'bower_components/angular/angular.js',
    'bower_components/momentjs/moment.js',
    'bower_components/angular-ui-router/release/angular-ui-router.js',
    'bower_components/angular-resource/angular-resource.js',
    'bower_components/angular-local-storage/dist/angular-local-storage.js',
    'bower_components/smalot-bootstrap-datetimepicker/js/bootstrap-datetimepicker.js',
    'bower_components/moment/min/moment.min.js',
    'bower_components/angular-ui-calendar/src/calendar.js',
    'bower_components/fullcalendar/dist/fullcalendar.min.js',
    'bower_components/fullcalendar/dist/gcal.js',
    'bower_components/angucomplete-alt/angucomplete-alt.js',
    'bower_components/bootstrap-select/js/bootstrap-select.js',
    'bower_components/circular-json/build/circular-json.js',
    'bower_components/angular-ui-select/dist/select.js',
    'bower_components/angular-notification-icons/dist/angular-notification-icons.min.js',
    'bower_components/ng-notify/dist/ng-notify.min.js'
];


/**
*   The location of the resources for deploy
*/
var DESTINATION = 'dest/';

var FONTS_DESTINATION = 'fonts/';
/**
* The single page initial html file. It will be altered
* by this script.
*/
var INDEX_FILE = 'index.html';
/**
* The name of the angular module
*/
var  MODULE_NAME = 'meet-me';
/**
* The URL of the back-end API
*/
var API_URL = 'https://localhost:44362/api';
/**
* Route to which the API calls will be mapped
*/
var API_ROUTE = '/api';

/**
* Task for concatenation of the js libraries used
* in this project
*/
gulp.task('concat_js_lib', function () {
    return gulp.src(JS_LIB) // which js files
        .pipe(concat('lib.js')) // concatenate them in lib.js
        .pipe(gulp.dest(DESTINATION)); // save lib.js in the DESTINATION folder
});

/**
* Task for concatenation of the css libraries used
* in this project
*/
gulp.task('concat_css_lib', function () {
    return gulp.src(CSS_LIB) // which css files
        .pipe(concat('lib.css')) // concat them in lib.css
        .pipe(gulp.dest(DESTINATION)); // save lib.css in the DESTINATION folder
});

/**
* Task for concatenation of the js code defined
* in this project
*/
gulp.task('concat_js_app', function () {
    return gulp.src(JS_APP)
        .pipe(concat('src.js'))
        .pipe(gulp.dest(DESTINATION))
});

/**
* Task for concatenation of the css code defined
* in this project
*/
gulp.task('concat_css_app', function () {
    return gulp.src(CSS_APP)
        .pipe(concat('app.css'))
        .pipe(gulp.dest(DESTINATION))
});

/**
* Task for concatenation of the html templates defined
* in this project
*/
gulp.task('templates', function () {
    return gulp.src(TEMPLATES_SRC) // which html files
        .pipe(
            templateCache('templates.js', { // compile them as angular templates
                module: MODULE_NAME,        // from module MODULE_NAME
                root: 'app'                 // of the app
            }))
        .pipe(gulp.dest(DESTINATION));
});

/**
* Task for adding the revision as parameter
* for cache braking
*/
gulp.task('cache-break', function () {
    return gulp.src(INDEX_FILE) // use the INDEX_FILE as source
        .pipe(rev())            // append the revision to all resources
        .pipe(gulp.dest('.'));  // save the modified file at the same destination
});

gulp.task('fonts', function () {
    return gulp.src(FONTS_LIB)
      .pipe(gulp.dest(FONTS_DESTINATION))
});

var tasks = [
    'concat_js_lib',
    'concat_css_lib',
    'concat_js_app',
    'concat_css_app',
    'fonts',
    'templates'
];

gulp.task('build', tasks, function () {
    gulp.start('cache-break');
});

gulp.task('watch', function () {
    gulp.watch('app/**/**.js', ['concat_js_app', 'cache-break']);
    gulp.watch('app/**/**.html', ['templates', 'cache-break']);
    gulp.watch('css/**/**.css', ['concat_css_app', 'cache-break']);
});

gulp.task('serve', function () {
    connect.server({
        port: 8000,
        livereload: true,
        middleware: function (connect, opt) {
            return [
                (function () {
                    var url = require('url');
                    var proxy = require('proxy-middleware');
                    var options = url.parse(API_URL);
                    options.route = API_ROUTE;
                    return proxy(options);
                })()
            ];
        }
    });
});

gulp.task('default', ['serve', 'watch']);
