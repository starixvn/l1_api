const fs = require('fs').promises;
const path = require('path');

class CollectionLoader {
    static async loadCollections() {
        const collections = {
            authentication: {}, // for enter-portal.postman_collection.json
            balance: {},       // for get-balance and get-bet-limit
            betting: {},        // Ensure game flow order
            special: {}        // for hit-jackpot and promotion-award
        };

        const collectionTypes = Object.keys(collections);
        
        for (const type of collectionTypes) {
            const typePath = path.join(__dirname, '..', 'collections', type);
            try {
                if (type === 'betting') {
                    await this.loadBettingCollections(collections.betting, typePath);
                } else {
                    const files = await fs.readdir(typePath);
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
        const bettingFlow = {
            'place-bet': /placebet/i,
            'end-game': /endgame/i,
            'void-game': /voidgame/i
        };
        
        try {
            const files = await fs.readdir(typePath);
            const collections = {};

            // First, load all collection files
            for (const file of files) {
                if (file.endsWith('.postman_collection.json')) {
                    const content = await fs.readFile(path.join(typePath, file));
                    const collection = JSON.parse(content);
                    collections[file] = collection;
                }
            }

            // Then, categorize collections based on their URLs
            for (const [flowKey, urlPattern] of Object.entries(bettingFlow)) {
                bettingCollections[flowKey] = this.findCollectionByUrl(collections, urlPattern);
            }

        } catch (error) {
            console.warn(`Warning: Error loading betting collections: ${error.message}`);
        }

        // Remove null collections
        Object.keys(bettingCollections).forEach(key => {
            if (bettingCollections[key] === null) {
                delete bettingCollections[key];
            }
        });
    }

    static findCollectionByUrl(collections, urlPattern) {
        for (const [fileName, collection] of Object.entries(collections)) {
            if (this.hasMatchingRequest(collection, urlPattern)) {
                return collection;
            }
        }
        return null;
    }

    static hasMatchingRequest(collection, urlPattern) {
        if (!collection.item) return false;

        const hasMatch = collection.item.some(item => {
            if (item.request?.url) {
                const url = typeof item.request.url === 'string' 
                    ? item.request.url 
                    : item.request.url.raw || item.request.url.path?.join('/');
                
                return urlPattern.test(url) || urlPattern.test(item.name);
            }
            return false;
        });

        return hasMatch;
    }
}

module.exports = { CollectionLoader }; 