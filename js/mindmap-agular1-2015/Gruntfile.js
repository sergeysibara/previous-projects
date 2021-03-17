module.exports = function (grunt) {

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        clean: ["dist/**"],

        concat: {
            js: {
                src: 'src/app/**/*compiled.js',
                dest: 'dist/app/dist.js'
            },
            css: {
                src: 'src/app/content/styles/*.css',
                dest: 'dist/app/dist.css'
            }
        },

        ngAnnotate: {
            options: {
            },
            app2: {
                files: [
                    {
                        expand: true,
                        src: 'src/app/**/*.js',
                        ext: '.js',
                        extDot: 'last'
                    }
                ]
            }
        },

        copy: {
            main: {
                files: [
                    {
                        expand: true,
                        cwd: 'src/',
                        src: ['**', '!**/*.js', '!**/*.css', '!**/*.scss', '!**/*.map'],
                        dest: 'dist/'
                    },
                    {
                        expand: true,
                        cwd: 'src/libraries/',
                        src: ['**'],
                        dest: 'dist/libraries/'
                    }
                ]
            }
        },

        dom_munger: {
            your_target: {
                options: {
                    append: [
                        {selector: 'head', html: '    <link rel="stylesheet" href="dist.css">'},
                        {selector: 'body', html: '    <script src="dist.js"></script>'}
                    ]
                },
                src: 'src/app/index.html',
                dest: 'dist/app/index.html'
            }
        },

        lineremover: {
            src: {
                files: {
                    'dist/app/index.html': 'dist/app/index.html'
                },
                options: {
                    exclusionPattern: /-compiled| href="content/g
                }
            },
            empty: {
                files: {
                    'dist/app/index.html': 'dist/app/index.html'
                }
            }
        },

        bower: {
            install: {
                options: {
                    targetDir: 'src/libraries',
                    layout: 'byType',
                    install: true,
                    verbose: false,
                    cleanTargetDir: false,
                    cleanBowerDir: false,
                    bowerOptions: {}
                }
            }
        }

    });

    grunt.loadNpmTasks('grunt-bower-task');

    grunt.loadNpmTasks('grunt-ng-annotate');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-dom-munger');
    grunt.loadNpmTasks('grunt-line-remover');

    grunt.registerTask('default', ['ngAnnotate', 'clean', 'copy', 'concat', 'dom_munger', 'lineremover']);
};