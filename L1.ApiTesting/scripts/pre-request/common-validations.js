// Pre-request Script
pm.environment.set('current_timestamp', new Date().toISOString());

// Validate required environment variables
const requiredVars = ['operatorId', 'playerId', 'siteId'];
requiredVars.forEach(variable => {
    if (!pm.environment.get(variable)) {
        throw new Error(`Required environment variable '${variable}' is not set`);
    }
}); 