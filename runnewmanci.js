const newman = require('newman');
const fs = require('fs');
const csv = require('csv-parser');

// Constants
const CONFIG = {
    LOBBY_ENDPOINTS: ['EnterPortal', '/GetBalance', '/GetBetLimit', '/Award'],
    GAME_ENDPOINTS: ['/PlaceBet', '/EndGame', '/VoidGame', '/HitJackpot'],
    JACKPOT_GAME_IDS: ['29', '70', '74', '76'],
    IS_CI: process.env.CI || false,
    JUNIT_REPORT_NAME: 'junit-report.xml'
};

// Report Configuration
class ReportConfig {
    static getReporterConfig(reportName) {
        const reporters = ['cli'];
        const reporterOptions = {};

        if (CONFIG.IS_CI) {
            reporters.push('junit');
            reporterOptions.junit = {
                export: `${reportName}-${CONFIG.JUNIT_REPORT_NAME}`
            };
        }

        return { reporters, reporterOptions };
    }
}

// Collection Handler
class CollectionHandler {
    static filterByEndpoints(collection, patterns) {
        const filteredCollection = JSON.parse(JSON.stringify(collection));
        filteredCollection.item = collection.item.filter(item => 
            patterns.some(pattern => item.request.url.raw.includes(pattern))
        );
        return filteredCollection;
    }

    static filterGameEndpoints(collection, skipVoidGame, gameId) {
        let endpoints = skipVoidGame ? 
            ['/PlaceBet', '/EndGame'] : 
            ['/PlaceBet', '/EndGame', '/VoidGame'];

        if (CONFIG.JACKPOT_GAME_IDS.includes(gameId)) {
            endpoints.push('/HitJackpot');
        }

        return this.filterByEndpoints(collection, endpoints);
    }
}

// Environment Handler
class EnvironmentHandler {
    static getUrls(environment) {
        return environment.values
            .filter(v => v.key.endsWith('_url') && v.enabled)
            .map(v => ({
                key: v.key,
                value: v.value,
                isL2: v.key === 'l2_url'
            }));
    }

    static createUrlEnvironment(baseEnv, urlConfig) {
        const newEnv = JSON.parse(JSON.stringify(baseEnv));
        newEnv.values.forEach(v => {
            if (v.key.endsWith('_url')) {
                v.enabled = false;
            }
        });
        const currentUrl = newEnv.values.find(v => v.key === urlConfig.key);
        if (currentUrl) {
            currentUrl.enabled = true;
        }
        return newEnv;
    }

    static createGameEnvironment(baseEnv, game, isL2Url) {
        const gameEnvironment = JSON.parse(JSON.stringify(baseEnv));
        
        // Update gameId
        const gameIdValue = gameEnvironment.values.find(v => v.key === 'gameId');
        if (gameIdValue) {
            gameIdValue.value = game.gameId;
        }

        if (isL2Url) {
            this.addL2UrlParameters(gameEnvironment, game);
        }

        return gameEnvironment;
    }

    static addL2UrlParameters(environment, game) {
        const l2Parameters = [
            { key: 'contributionRate', value: game.contributionRate },
            { key: 'contributionAmount', value: game.contributionAmount }
        ];

        l2Parameters.forEach(param => {
            const existingIndex = environment.values.findIndex(v => v.key === param.key);
            if (existingIndex >= 0) {
                environment.values[existingIndex].value = param.value;
            } else {
                environment.values.push({
                    key: param.key,
                    value: param.value,
                    type: 'default',
                    enabled: true
                });
            }
        });
    }
}

// Test Runner
class TestRunner {
    static async runNewman(collection, environment, reportName) {
        const { reporters, reporterOptions } = ReportConfig.getReporterConfig(reportName);

        return new Promise((resolve, reject) => {
            newman.run({
                collection,
                environment,
                reporters,
                reporter: reporterOptions,
                bail: CONFIG.IS_CI,
                suppressExitCode: !CONFIG.IS_CI
            }, (err, summary) => {
                if (err) {
                    reject(err);
                    return;
                }

                const failures = summary.run.failures;
                if (failures.length > 0) {
                    reject({
                        message: 'Test failures detected',
                        failures: failures.map(f => ({
                            name: f.source.name,
                            error: f.error.message,
                            test: f.parent.name
                        }))
                    });
                } else {
                    resolve(summary);
                }
            });
        });
    }

    static async runLobbyTests(collection, urlConfig, urlEnvironment) {
        const lobbyCollection = CollectionHandler.filterByEndpoints(collection, CONFIG.LOBBY_ENDPOINTS);
        const lobbyReportName = `lobby-apis-${urlConfig.key.replace('_url', '')}`;
        
        try {
            await this.runNewman(lobbyCollection, urlEnvironment, lobbyReportName);
            console.log(`✓ Lobby APIs completed successfully for ${urlConfig.key}`);
        } catch (error) {
            console.error(`\n❌ Lobby APIs Failed for ${urlConfig.key}`);
            ErrorHandler.displayError(error);
            process.exit(1);
        }
    }

    static async runGameTests(collection, game, urlConfig, urlEnvironment) {
        const gameEnvironment = EnvironmentHandler.createGameEnvironment(urlEnvironment, game, urlConfig.isL2);
        const reportName = Utilities.createSafeReportName(
            game.name, 
            game.gameId, 
            `${urlConfig.key.replace('_url', '')}-`
        );

        const skipVoidGame = game.type === '2' || game.type === '3';
        const gameCollection = CollectionHandler.filterGameEndpoints(collection, skipVoidGame, game.gameId);
        
        if (skipVoidGame) {
            console.log(`Note: VoidGame will be skipped for game type ${game.type}`);
        }
        if (CONFIG.JACKPOT_GAME_IDS.includes(game.gameId)) {
            console.log(`Note: HitJackpot will be included for game ID ${game.gameId}`);
        }

        try {
            await this.runNewman(gameCollection, gameEnvironment, reportName);
            console.log(`✓ Game tests completed successfully for ${game.name} (ID: ${game.gameId})`);
        } catch (error) {
            console.error(`\n❌ Game tests failed for ${game.name} (ID: ${game.gameId})`);
            ErrorHandler.displayError(error, {
                ...game,
                environmentUrl: urlConfig.key,
                isL2: urlConfig.isL2,
                contributionRate: urlConfig.isL2 ? game.contributionRate : 'N/A',
                contributionAmount: urlConfig.isL2 ? game.contributionAmount : 'N/A'
            });
            process.exit(1);
        }
    }
}

// Error Handler
class ErrorHandler {
    static displayError(error, gameInfo = null) {
        const errorMessage = [
            '\n=== TEST FAILURE REPORT ===',
            `Error: ${error.message}`
        ];

        if (gameInfo) {
            this.addGameInfo(errorMessage, gameInfo);
        }

        if (error.failures) {
            this.addFailureDetails(errorMessage, error);
        }

        errorMessage.push('\n=========================');

        const logMethod = CONFIG.IS_CI ? console.error : console.log;
        logMethod(errorMessage.join('\n'));
    }

    static addGameInfo(errorMessage, gameInfo) {
        errorMessage.push(
            '\nGame Information:',
            `Game Name: ${gameInfo.name}`,
            `Game ID: ${gameInfo.gameId}`,
            `Game Type: ${gameInfo.type}`
        );

        if (gameInfo.isL2) {
            errorMessage.push(
                `Contribution Rate: ${gameInfo.contributionRate}`,
                `Contribution Amount: ${gameInfo.contributionAmount}`
            );
        }
    }

    static addFailureDetails(errorMessage, error) {
        errorMessage.push('\nFailure Details:');
        error.failures.forEach((failure, index) => {
            errorMessage.push(
                `\n${index + 1}. Failed Request: ${failure.name}`,
                `   Test Failed: ${failure.test}`,
                `   Error: ${failure.error}`
            );
        });
    }
}

// Utilities
class Utilities {
    static createSafeReportName(gameName, gameId, prefix = '') {
        const safeName = gameName.replace(/[^a-zA-Z0-9]/g, '-').toLowerCase();
        return `${prefix}${safeName}-${gameId}`;
    }

    static async readGamesFromCSV() {
        const games = [];
        await new Promise((resolve, reject) => {
            fs.createReadStream('games.csv')
                .pipe(csv())
                .on('data', (data) => games.push(data))
                .on('end', resolve)
                .on('error', reject);
        });
        return games;
    }
}

// Main execution
async function main() {
    try {
        const collection = require('./Upgrade.Net8.postman_collection.json');
        const environment = require('./Local.postman_environment.json');

        const envUrls = EnvironmentHandler.getUrls(environment);
        console.log(`Found ${envUrls.length} environment URLs to test`);

        for (const urlConfig of envUrls) {
            console.log(`\nTesting with ${urlConfig.key}...`);
            const urlEnvironment = EnvironmentHandler.createUrlEnvironment(environment, urlConfig);

            await TestRunner.runLobbyTests(collection, urlConfig, urlEnvironment);

            const games = await Utilities.readGamesFromCSV();
            for (const game of games) {
                console.log(`\nProcessing game: ${game.name} (ID: ${game.gameId}) for ${urlConfig.key}`);
                await TestRunner.runGameTests(collection, game, urlConfig, urlEnvironment);
            }
        }

        console.log('\n✅ All environment tests completed successfully!');
        if (CONFIG.IS_CI) {
            console.log('Test execution completed in CI environment');
        }
    } catch (error) {
        console.error('\n❌ Test execution failed');
        ErrorHandler.displayError(error);
        process.exit(1);
    }
}

// Run the application
main();