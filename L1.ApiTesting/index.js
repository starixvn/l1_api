const path = require('path');
const { CollectionLoader } = require('./lib/collection-loader');
const { EnvironmentLoader } = require('./lib/environment-loader');
const { TestRunner } = require('./lib/test-runner');

async function main() {
    try {
        const collections = await CollectionLoader.loadCollections();
        const environments = await EnvironmentLoader.loadEnvironments();

        const config = {
            reportsPath: path.join(__dirname, 'reports')
        };

        const runner = new TestRunner(collections, environments, config);
        await runner.runAll();

        console.log('\n✅ All tests completed successfully!');
    } catch (error) {
        console.error('\n❌ Test execution failed');
        console.error(error);
        process.exit(1);
    }
}

main();