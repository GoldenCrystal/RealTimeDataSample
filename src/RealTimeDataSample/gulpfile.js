/// <binding Clean='clean' />

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  fs = require("fs");

eval("var project = " + fs.readFileSync("./project.json"));

var paths = {
  bower: "./bower_components/",
  lib: "./" + project.webroot + "/lib/"
};

gulp.task("clean:lib", function (cb) {
  rimraf(paths.lib, cb);
});

gulp.task("copy:lib", ["clean:lib"], function () {
  var bower = {
    "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "jquery": "jquery/jquery*.{js,map}",
    "d3": "d3/d3*.js",
    "signalr": "signalr/jquery.signalR*.js",
  }

  for (var destinationDir in bower) {
    gulp.src(paths.bower + bower[destinationDir])
      .pipe(gulp.dest(paths.lib + destinationDir));
  }
});

gulp.task("clean", ["clean:lib"]);
gulp.task("copy", ["copy:lib"]);
