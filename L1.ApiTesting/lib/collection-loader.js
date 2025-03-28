const fs = require('fs').promises;
const path = require('path');

class CollectionLoader {
    static async loadCollections() {
        const collections = {
            authentication: {}, // for enter-portal.postman_collection.json
            balance: {},       // for get-balance and get-bet-limit
            betting: {         // Ensure game flow order
            },
            special: {}        // for hit-jackpot and promotion-award
        };

        const collectionTypes = Object.keys(collections);
        
        for (const type of collectionTypes) {
            const typePath = path.join(__dirname, '..', 'collections', type);
            try {
                const files = await fs.readdir(typePath);
                
                if (type === 'betting') {
                    // Special handling for betting to ensure order
                    await this.loadBettingCollections(collections.betting, typePath);
                } else {
                    // Normal handling for other types
                    for (const file of files) {
                        if (file.endsWith('.postman_collection.json')) {
                            const content = await fs.readFile(path.join(typePath, file));
                            const collectionName = file.replace('.postman_collection.json', '');
                            collections[type][collectionName] = JSON.parse(content);
                        }
                    }
                }
            } catch (error) {
                console.warn(`Warning: Could not load collections from ${type}: ${error.message}`);
            }
        }

        // Log to check loaded collections
        console.log('\nLoaded Collections:');
        for (const [type, typeCollections] of Object.entries(collections)) {
            console.log(`\n${type}:`);
            Object.keys(typeCollections).forEach(name => {
                console.log(`  - ${name}`);
            });
        }

        return collections;
    }

    static async loadBettingCollections(bettingCollections, typePath) {
        const bettingFlow = ['place-bet', 'end-game', 'void-game'];
        
        for (const collectionName of bettingFlow) {
            const fileName = `${collectionName}.postman_collection.json`;
            const filePath = path.join(typePath, fileName);
            
            try {
                const content = await fs.readFile(filePath);
                bettingCollections[collectionName] = JSON.parse(content);
            } catch (error) {
                console.warn(`Warning: Could not load betting collection ${fileName}: ${error.message}`);
                bettingCollections[collectionName] = null;
            }
        }

        // Remove null collections
        Object.keys(bettingCollections).forEach(key => {
            if (bettingCollections[key] === null) {
                delete bettingCollections[key];
            }
        });
    }
}

module.exports = { CollectionLoader }; 