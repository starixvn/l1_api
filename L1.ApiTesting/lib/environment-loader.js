const fs = require('fs').promises;
const path = require('path');

class EnvironmentLoader {
    static async loadEnvironments(environmentName) {
        const environments = {};
        const envPath = path.join(__dirname, '..', 'environments');
        
        try {
            const envTypes = await fs.readdir(envPath);
            
            for (const envType of envTypes) {
                const envTypePath = path.join(envPath, envType);
                const stats = await fs.stat(envTypePath);
                
                if (stats.isDirectory()) {
                    const files = await fs.readdir(envTypePath);
                    
                    for (const file of files) {
                        if (file.endsWith('.postman_environment.json')) {
                            const content = await fs.readFile(path.join(envTypePath, file));
                            const envName = `${envType}-${file.replace('.postman_environment.json', '')}`;
                            environments[envName] = JSON.parse(content);
                        }
                    }
                }
            }

            if(environmentName) {
                return { [environmentName]: environments[environmentName] };
            }
        } catch (error) {
            console.error(`Error loading environments: ${error.message}`);
            throw error;
        }
        
        return environments;
    }
}

module.exports = { EnvironmentLoader }; 