const gulp = require('gulp')
      childProcess = require('child_process');

/////////////////////////////////////
//  DOTNET CLEAN TASKS
/////////////////////////////////////
// Clean .net solution
const clean = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('dotnet', [
        'clean',
        'FutureNHS.Data\\FutureNHS.Data.FutureNHS\\FutureNHS.Data.FutureNHS.sqlproj',
        '/p:NetCoreBuild=true',
        '/p:SystemDacpacsLocation="FutureNHS.Data\\ExperimentalBuildTools"'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when cleaning project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error cleaning project'));
        }

        return done();
    });
};


/////////////////////////////////////
//  DOTNET BUILD TASKS
/////////////////////////////////////
// Build .net solution
const dotnetBuild = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('dotnet', [
        'build',
        'FutureNHS.Data\\FutureNHS.Data.FutureNHS\\FutureNHS.Data.FutureNHS.sqlproj',
        '/p:NetCoreBuild=true',
        '/p:SystemDacpacsLocation="FutureNHS.Data\\ExperimentalBuildTools"'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when building project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error compiling project'));
        }

        return done();
    });
};

// Build task - runs all the build tasks
const build = gulp.series(clean, dotnetBuild);

///////////////////////////////////////
//  FutureNHS DB TASKS
//////////////////////////////////////

const deployFutureNHSDatabase = (done) => {
    process.env.PATH = `${process.env.PATH}`;
    console.log(process.env.PATH);
    
    const sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        `/SourceFile:FutureNHS.Data/FutureNHS.Data.FutureNHS/bin/Debug/FutureNHS.Data.FutureNHS.dacpac`,
        '/TargetDatabaseName:FutureNHS',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:xFQF$a9I78uY',
        `/DeployReportPath:FutureNHS.Data/FutureNHS.Data.FutureNHS/Report.xml`,
        `/DeployScriptPath:FutureNHS.Data/FutureNHS.Data.FutureNHS/Publish.sql`,
        `/Profile:FutureNHS.Data/FutureNHS.Data.FutureNHS/FutureNHS.Data.FutureNHS.publish.xml`,
    ], {
        cwd: process.cwd(), 
      
    });

    sqlPackage.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stdout.on('close', () => {
        return done();
    })
};

const deployAutomationFutureNHSDatabase = (done) => {
    process.env.PATH = `${process.env.PATH}`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
	'/Diagnostics:True',
        '/SourceFile:FutureNHS.Data/FutureNHS.Data.FutureNHS/bin/Debug/FutureNHS.Data.FutureNHS.dacpac',
        '/TargetDatabaseName:FutureNHS',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:xFQF$a9I78uY',
        '/DeployReportPath:FutureNHS.Data/FutureNHS.Data.FutureNHS/Report.xml',
        '/DeployScriptPath:FutureNHS.Data/FutureNHS.Data.FutureNHS/Publish.sql',
        '/Profile:FutureNHS.Data/FutureNHS.Data.FutureNHS/FutureNHS.Data.FutureNHS-automated.publish.xml',
    ], {
        cwd: process.cwd()
    });
console.log(process.env.PATH);
    sqlPackage.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stdout.on('close', () => {
        return done();
    });
};


const dropFutureNHSDatabase = (done) => {
    var sqlCmd = childProcess.spawn('sqlcmd', [
        '-U',
        'sa',
        '-P',
        'xFQF$a9I78uY',
        '-Q',
        'DROP DATABASE FutureNHS',
    ], {
        cwd: process.cwd()
    });

    sqlCmd.stdout.on('data', (data) => {
        console.log(data.toString())
    })

    sqlCmd.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error dropping database'));
        }

        return done();
    });
};


module.exports = {
    build,
    deployFutureNHSDatabase,
    deployAutomationFutureNHSDatabase
}