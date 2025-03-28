const newman = require('newman');
const path = require('path');
const fs = require('fs').promises;
const csv = require('csv-parser');
const { createReadStream } = require('fs');

class TestRunner {
    constructor(collections, environments, config) {
        this.collections = collections;
        this.environments = environments;
        this.config = config;
        this.LOBBY_ENDPOINTS = ['EnterPortal', 'Balance', 'GetBetLimit', 'Award'];
        this.GAME_ENDPOINTS = {
            PLACE_BET: 'PlaceBet',
            END_GAME: 'EndGame',
            VOID_GAME: 'VoidGame',
            HIT_JACKPOT: 'HitJackpot'
        };
        this.JACKPOT_GAME_IDS = ['29', '70', '74', '76'];
    }

    static getReporterConfig(reportName) {
        const reporters = ['cli'];
        const reporterOptions = {
            htmlextra: {
                export: path.join(__dirname, '..', 'reports', `${reportName}.html`),
                title: `Test Report - ${reportName}`
            }
        };

        if (process.env.CI) {
            reporters.push('junit');
            reporterOptions.junit = {
                export: path.join(__dirname, '..', 'reports', `${reportName}-junit-report.xml`)
            };
        }

        reporters.push('htmlextra');
        return { reporters, reporterOptions };
    }

    async runAll() {
        for (const [envName, environment] of Object.entries(this.environments)) {
            console.log(`\nTesting with environment: ${envName}`);
            await this.runLobbyTests(envName, environment);
            await this.runGameTests(envName, environment);
        }
    }

    async runLobbyTests(envName, environment) {
        for (const [collectionType, typeCollections] of Object.entries(this.collections)) {
            for (const [collectionName, collection] of Object.entries(typeCollections)) {
                const filteredCollection = this.filterByEndpoints(collection, this.LOBBY_ENDPOINTS);
                if (filteredCollection.item.length > 0) {
                    const reportName = `lobby-${envName}-${collectionName}`;
                    await this.runTest(filteredCollection, environment, reportName);
                }
            }
        }
    }

    async runGameTests(envName, environment) {
        const games = await this.readGamesFromCSV();
        
        for (const game of games) {
            console.log(`\nProcessing game: ${game.name} (ID: ${game.gameId}) for ${envName}`);
            
            const gameEnvironment = this.createGameEnvironment(environment, game);
            
            for (const [collectionType, typeCollections] of Object.entries(this.collections)) {
                for (const [collectionName, collection] of Object.entries(typeCollections)) {
                    const skipVoidGame = game.type === '2' || game.type === '3';
                    const filteredCollection = this.filterGameEndpoints(collection, skipVoidGame, game.gameId);
                    
                    if (filteredCollection.item.length > 0) {
                        const reportName = this.createSafeReportName(game.name, game.gameId, `${envName}-${collectionName}-`);
                        await this.runTest(filteredCollection, gameEnvironment, reportName);
                    }
                }
            }
        }
    }

    async runTest(collection, environment, reportName) {
        const { reporters, reporterOptions } = TestRunner.getReporterConfig(reportName);

        return new Promise((resolve, reject) => {
            newman.run({
                collection: collection,
                environment: environment,
                reporters: reporters,
                reporter: reporterOptions,
                bail: process.env.CI,
                suppressExitCode: !process.env.CI
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
                    console.log(`✓ Tests completed successfully for ${reportName}`);
                    resolve(summary);
                }
            });
        });
    }

    filterByEndpoints(collection, patterns) {
        const filteredCollection = JSON.parse(JSON.stringify(collection));
        
        filteredCollection.item = collection.item.filter(item => {
            if (item.request?.url) {
                const urlPath = item.request.url.raw || item.request.url;
                return patterns.some(pattern => 
                    urlPath.toLowerCase().includes(pattern.toLowerCase()) ||
                    item.name.toLowerCase().includes(pattern.toLowerCase())
                );
            }
            return false;
        });

        return filteredCollection;
    }

    filterGameEndpoints(collection, skipVoidGame, gameId) {
        let endpoints = skipVoidGame ? 
            [this.GAME_ENDPOINTS.PLACE_BET, this.GAME_ENDPOINTS.END_GAME] : 
            [this.GAME_ENDPOINTS.PLACE_BET, this.GAME_ENDPOINTS.END_GAME, this.GAME_ENDPOINTS.VOID_GAME];

        if (this.JACKPOT_GAME_IDS.includes(gameId)) {
            endpoints.push(this.GAME_ENDPOINTS.HIT_JACKPOT);
        }

        return this.filterByEndpoints(collection, endpoints);
    }

    createGameEnvironment(baseEnv, game) {
        const gameEnvironment = JSON.parse(JSON.stringify(baseEnv));
        
        const gameIdValue = gameEnvironment.values.find(v => v.key === 'gameId');
        if (gameIdValue) {
            gameIdValue.value = game.gameId;
        } else {
            gameEnvironment.values.push({
                key: 'gameId',
                value: game.gameId,
                type: 'default',
                enabled: true
            });
        }

        return gameEnvironment;
    }

    createSafeReportName(gameName, gameId, prefix = '') {
        return `${prefix}${gameName}-${gameId}`;
    }

    async readGamesFromCSV() {
        const games = [];
        const csvPath = path.join(__dirname, '..', 'data', 'games.csv');
        
        return new Promise((resolve, reject) => {
            createReadStream(csvPath)
                .pipe(csv())
                .on('data', (data) => games.push(data))
                .on('end', () => resolve(games))
                .on('error', (error) => {
                    console.error(`Error reading CSV file: ${error.message}`);
                    reject(error);
                });
        });
    }
}

module.exports = { TestRunner }; 