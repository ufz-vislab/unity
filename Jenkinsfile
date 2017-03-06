#!/usr/bin/env groovy

node('docker') {
    checkout scm
    def image = docker.image('ogs6/gcc-latex:latest')
    image.pull()
    image.inside() {
        stage('Generate Docs') {
            sh "doxygen"
        }
    }

    stage('Publish Docs') {
        publishHTML(target: [allowMissing: false, alwaysLinkToLastBuild: false,
            keepAll: false, reportDir: '.docs', reportFiles: 'index.html',
            reportName: 'Doxygen'])
    }
}
