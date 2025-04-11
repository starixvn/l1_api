# MongoDB Schema for FreeSpin Promotions

## Collections

### 1. Promotions Collection
```javascript
{
    _id: ObjectId,                    // PromotionId
    name: String,                     // PromotionName
    operatorId: String,               // Operator identifier
    siteIds: [String],                // Array of site identifiers
    startDate: ISODate,              // Start date and time
    endDate: ISODate,                // End date and time
    freeSpinCount: Number,           // Number of free spins per player
    isAllPlayers: Boolean,           // Whether promotion is for all players
    playerIds: [String],             // List of eligible player IDs (if not all players)
    games: [                         // List of eligible games
        {
            gameId: Number,
            gameName: String,
            gameType: String         // "ClassicSlot" | "CascadingSlot"
        }
    ],
    createdAt: ISODate,
    updatedAt: ISODate
}
```

### 2. PlayerPromotions Collection
```javascript
{
    _id: ObjectId,
    playerId: String,
    operatorId: String,              // Operator identifier
    siteId: String,                  // Site identifier (single site per player promotion)
    promotionId: ObjectId,           // Reference to Promotions collection
    remainingSpins: Number,          // 0 means promotion is completed
    lastUsedDate: ISODate,
    createdAt: ISODate,
    updatedAt: ISODate
}
```

## Example Queries

### 1. Check Player's Active Promotion
```javascript
db.playerPromotions.findOne({
    operatorId: "op123",
    siteId: "site456",
    playerId: "player123",
    remainingSpins: { $gt: 0 }  // Active promotion has remaining spins
})
```

### 2. Find Available Promotions
```javascript
db.promotions.find({
    operatorId: "op123",
    siteIds: "site456",  // Will match if site456 is in the siteIds array
    startDate: { $lte: new Date() },
    endDate: { $gt: new Date() }
})
```

### 3. Check Player Eligibility
```javascript
// For specific player list promotions
db.promotions.find({
    operatorId: "op123",
    siteIds: "site456",  // Will match if site456 is in the siteIds array
    startDate: { $lte: new Date() },
    endDate: { $gt: new Date() },
    isAllPlayers: false,
    playerIds: "player123"
})

// For all players promotions
db.promotions.find({
    operatorId: "op123",
    siteIds: "site456",  // Will match if site456 is in the siteIds array
    startDate: { $lte: new Date() },
    endDate: { $gt: new Date() },
    isAllPlayers: true
})
```

### 4. Check Game Availability
```javascript
db.promotions.find({
    operatorId: "op123",
    siteIds: "site456",  // Will match if site456 is in the siteIds array
    startDate: { $lte: new Date() },
    endDate: { $gt: new Date() },
    "games.gameId": gameId
})
``` 