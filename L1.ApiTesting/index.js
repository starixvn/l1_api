const path = require('path');
const yargs = require('yargs');
const { CollectionLoader } = require('./lib/collection-loader');
const { EnvironmentLoader } = require('./lib/environment-loader');
const { TestRunner } = require('./lib/test-runner');

async function main() {
    try {
        const argv = yargs.option('env', {
            alias: 'e',
            describe: 'Specify the environment',
            type: 'string',
            demandOption: false 
        }).argv;

        const environmentName = argv.env;
        console.log(`\n🚀 Running tests for environment: ${environmentName}`);

        const collections = await CollectionLoader.loadCollections();
        const environment = await EnvironmentLoader.loadEnvironments(environmentName);

        if (!environment) {
            throw new Error(`Environment '${environmentName}' not found.`);
        }

        const config = {
            reportsPath: path.join(__dirname, 'reports')
        };

        const runner = new TestRunner(collections, environment, config);
        await runner.runAll();

        console.log('\n✅ All tests completed successfully!');
    } catch (error) {
        console.error('\n❌ Test execution failed');
        console.error(error);
        process.exit(1);
    }
}

main();
