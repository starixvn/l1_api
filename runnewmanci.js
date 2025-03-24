const newman = require('newman');
const fs = require('fs');
const csv = require('csv-parser');

// Đọc collection và environment
const collection = require('./Upgrade.Net8.postman_collection.json');
const environment = require('./Local.postman_environment.json');

// Định nghĩa các endpoint
const LOBBY_ENDPOINTS = ['EnterPortal' ,'/GetBalance', '/GetBetLimit'];
const GAME_ENDPOINTS = ['/PlaceBet', '/EndGame', '/VoidGame'];

// Thêm các constants cho CI/CD
const IS_CI = process.env.CI || false;
const REPORTS_DIR = './reports';
const JUNIT_REPORT_NAME = 'junit-report.xml';

// Function để lọc request theo URL pattern
function filterRequestsByUrl(collection, patterns) {
    const filteredCollection = JSON.parse(JSON.stringify(collection));
    filteredCollection.item = collection.item.filter(item => 
        patterns.some(pattern => item.request.url.raw.includes(pattern))
    );
    return filteredCollection;
}

// Function để tạo tên file report an toàn
function createSafeReportName(gameName, gameId, prefix = '') {
    // Loại bỏ các ký tự không hợp lệ và thay thế bằng dấu gạch ngang
    const safeName = gameName.replace(/[^a-zA-Z0-9]/g, '-').toLowerCase();
    return `${prefix}${safeName}-${gameId}`;
}

// Function để chạy newman với promise và report chi tiết
function runNewman(collection, environment, reportName) {
    return new Promise((resolve, reject) => {
        const reporters = ['cli'];
        const reporterOptions = {};

        // Thêm reporters dựa vào môi trường
        if (IS_CI) {
            reporters.push('junit', 'htmlextra');
            reporterOptions.junit = {
                export: `${REPORTS_DIR}/${reportName}-${JUNIT_REPORT_NAME}`
            };
        } else {
            reporters.push('htmlextra');
        }

        reporterOptions.htmlextra = {
            export: `${REPORTS_DIR}/${reportName}.html`,
            title: reportName,
            browserTitle: "API Test Report",
            templateOptions: {
                showEnvironmentData: true,
                showGlobalData: true,
                showMarkdownLinks: true
            }
        };

        newman.run({
            collection: collection,
            environment: environment,
            reporters: reporters,
            reporter: reporterOptions,
            bail: IS_CI, // Dừng ngay khi fail trong CI
            suppressExitCode: !IS_CI // Không thoát với error code trong local
        }, function (err, summary) {
            if (err) {
                reject(err);
                return;
            }

            // Kiểm tra kết quả test
            const failures = summary.run.failures;
            if (failures.length > 0) {
                const error = {
                    message: 'Test failures detected',
                    failures: failures.map(f => ({
                        name: f.source.name,
                        error: f.error.message,
                        test: f.parent.name
                    })),
                    reportPath: `./reports/${reportName}.html`
                };
                reject(error);
            } else {
                resolve(summary);
            }
        });
    });
}

// Function để tạo environment cho game
function createGameEnvironment(baseEnv, game, isL2Url) {
    const gameEnvironment = JSON.parse(JSON.stringify(baseEnv));
    
    // Cập nhật gameId
    const gameIdValue = gameEnvironment.values.find(v => v.key === 'gameId');
    if (gameIdValue) {
        gameIdValue.value = game.gameId;
    }

    // Thêm contributionRate và contributionAmount cho l2_url
    if (isL2Url) {
        // Tìm giá trị l2_url trong environment
        const urlValue = gameEnvironment.values.find(v => v.key === 'l2_url');
        if (urlValue && urlValue.enabled) {
            // Thêm hoặc cập nhật contributionRate
            const contributionRateIndex = gameEnvironment.values.findIndex(v => v.key === 'contributionRate');
            if (contributionRateIndex >= 0) {
                gameEnvironment.values[contributionRateIndex].value = game.contributionRate;
            } else {
                gameEnvironment.values.push({
                    key: 'contributionRate',
                    value: game.contributionRate,
                    type: 'default',
                    enabled: true
                });
            }

            // Thêm hoặc cập nhật contributionAmount
            const contributionAmountIndex = gameEnvironment.values.findIndex(v => v.key === 'contributionAmount');
            if (contributionAmountIndex >= 0) {
                gameEnvironment.values[contributionAmountIndex].value = game.contributionAmount;
            } else {
                gameEnvironment.values.push({
                    key: 'contributionAmount',
                    value: game.contributionAmount,
                    type: 'default',
                    enabled: true
                });
            }
        }
    }

    return gameEnvironment;
}

// Function để hiển thị lỗi chi tiết
function displayError(error, gameInfo = null) {
    const errorMessage = [
        '\n=== TEST FAILURE REPORT ===',
        `Error: ${error.message}`
    ];

    if (gameInfo) {
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

    if (error.failures) {
        errorMessage.push('\nFailure Details:');
        error.failures.forEach((failure, index) => {
            errorMessage.push(
                `\n${index + 1}. Failed Request: ${failure.name}`,
                `   Test Failed: ${failure.test}`,
                `   Error: ${failure.error}`
            );
        });
        errorMessage.push(`\nDetailed HTML report available at: ${error.reportPath}`);
    }

    errorMessage.push('\n=========================');

    // Sử dụng console.error trong CI, console.log trong local
    const logMethod = IS_CI ? console.error : console.log;
    logMethod(errorMessage.join('\n'));
}

// Function để lấy danh sách environment URLs
function getEnvironmentUrls(environment) {
    return environment.values
        .filter(v => v.key.endsWith('_url') && v.enabled)
        .map(v => ({
            key: v.key,
            value: v.value,
            isL2: v.key === 'l2_url'
        }));
}

// Function để tạo environment cho mỗi URL
function createUrlEnvironment(baseEnv, urlConfig) {
    const newEnv = JSON.parse(JSON.stringify(baseEnv));
    // Disable tất cả các URL
    newEnv.values.forEach(v => {
        if (v.key.endsWith('_url')) {
            v.enabled = false;
        }
    });
    // Enable URL hiện tại
    const currentUrl = newEnv.values.find(v => v.key === urlConfig.key);
    if (currentUrl) {
        currentUrl.enabled = true;
    }
    return newEnv;
}

// Function chính để chạy test
async function runTests() {
    try {
        // Tạo thư mục reports
        if (!fs.existsSync(REPORTS_DIR)) {
            fs.mkdirSync(REPORTS_DIR, { recursive: true });
        }

        // Lấy danh sách URLs từ environment
        const envUrls = getEnvironmentUrls(environment);
        console.log(`Found ${envUrls.length} environment URLs to test`);

        // Chạy test cho từng environment URL
        for (const urlConfig of envUrls) {
            console.log(`\nTesting with ${urlConfig.key}...`);
            const urlEnvironment = createUrlEnvironment(environment, urlConfig);

            // 1. Chạy Lobby APIs
            try {
                const lobbyCollection = filterRequestsByUrl(collection, LOBBY_ENDPOINTS);
                const lobbyReportName = `lobby-apis-${urlConfig.key.replace('_url', '')}`;
                await runNewman(lobbyCollection, urlEnvironment, lobbyReportName);
            } catch (error) {
                console.error(`\nLobby APIs Failed for ${urlConfig.key}`);
                displayError(error);
                continue; // Tiếp tục với URL tiếp theo nếu lobby APIs fail
            }

            // 2. Đọc và chạy game tests
            const games = [];
            await new Promise((resolve, reject) => {
                fs.createReadStream('games.csv')
                    .pipe(csv())
                    .on('data', (data) => games.push(data))
                    .on('end', resolve)
                    .on('error', reject);
            });

            const gameCollection = filterRequestsByUrl(collection, GAME_ENDPOINTS);

            // Chạy test cho từng game
            for (const game of games) {
                console.log(`\nProcessing game: ${game.name} (ID: ${game.gameId}) for ${urlConfig.key}`);

                // Tạo environment cho game
                let gameEnvironment = JSON.parse(JSON.stringify(urlEnvironment));
                
                // Cập nhật gameId
                const gameIdValue = gameEnvironment.values.find(v => v.key === 'gameId');
                if (gameIdValue) {
                    gameIdValue.value = game.gameId;
                }

                // Thêm contribution values cho l2_url
                if (urlConfig.isL2) {
                    // Thêm contributionRate
                    gameEnvironment.values.push({
                        key: 'contributionRate',
                        value: game.contributionRate,
                        type: 'default',
                        enabled: true
                    });
                    // Thêm contributionAmount
                    gameEnvironment.values.push({
                        key: 'contributionAmount',
                        value: game.contributionAmount,
                        type: 'default',
                        enabled: true
                    });
                }

                // Tạo tên report
                const reportName = createSafeReportName(
                    game.name, 
                    game.gameId, 
                    `${urlConfig.key.replace('_url', '')}-`
                );

                // Skip VoidGame cho game type 2 hoặc 3
                const skipVoidGame = game.type === '2' || game.type === '3';
                if (skipVoidGame) {
                    console.log(`Note: VoidGame will be skipped for game type ${game.type}`);
                }

                try {
                    await runNewman(gameCollection, gameEnvironment, reportName);
                } catch (error) {
                    displayError(error, {
                        ...game,
                        environmentUrl: urlConfig.key,
                        isL2: urlConfig.isL2,
                        contributionRate: urlConfig.isL2 ? game.contributionRate : 'N/A',
                        contributionAmount: urlConfig.isL2 ? game.contributionAmount : 'N/A'
                    });
                    continue; // Tiếp tục với game tiếp theo nếu test fail
                }
            }
        }

        console.log('\nAll environment tests completed!');

        if (IS_CI) {
            console.log('Test execution completed in CI environment');
        }
    } catch (error) {
        displayError(error);
        if (IS_CI) {
            process.exit(1);
        }
    }
}

// Chạy tests
runTests();