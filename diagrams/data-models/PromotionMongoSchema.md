# MongoDB Schema for FreeSpin Promotions

## Collections

### 1. Promotions Collection
```javascript
{
    _id: ObjectId,                    // PromotionId
    name: String,                     // PromotionName
    code: String,                     // PromotionCode (derived from name, e.g., "VIP Classic Free Spins" => "vip-classic-free-spins")
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
    promotionCode: String,           // Promotion code for quick reference
    remainingSpins: Number,          // 0 means promotion is completed
    lastUsedDate: ISODate,
    createdAt: ISODate,
    updatedAt: ISODate
}
```

## Example Data

### 1. Promotion for All Players
```javascript
// Promotion document
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),
    name: "Summer Classic Free Spins",
    code: "summer-classic-free-spins",
    operatorId: "op123",
    siteIds: ["site456", "site789", "site101"],
    startDate: ISODate("2024-06-01T00:00:00Z"),
    endDate: ISODate("2024-06-30T23:59:59Z"),
    freeSpinCount: 50,
    isAllPlayers: true,
    playerIds: [],  // Empty as it's for all players
    games: [
        {
            gameId: 1001,
            gameName: "Classic Fruit",
            gameType: "ClassicSlot"
        },
        {
            gameId: 1002,
            gameName: "Lucky 7s",
            gameType: "ClassicSlot"
        }
    ],
    createdAt: ISODate("2024-05-15T10:00:00Z"),
    updatedAt: ISODate("2024-05-15T10:00:00Z")
}

// Example player promotion document
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j2"),
    playerId: "player123",
    operatorId: "op123",
    siteId: "site456",
    promotionId: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),
    promotionCode: "summer-classic-free-spins",
    remainingSpins: 50,
    lastUsedDate: null,
    createdAt: ISODate("2024-06-01T00:00:00Z"),
    updatedAt: ISODate("2024-06-01T00:00:00Z")
}
```

### 2. Promotion for Limited Players
```javascript
// Promotion document
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j3"),
    name: "VIP Classic Free Spins",
    code: "vip-classic-free-spins",
    operatorId: "op123",
    siteIds: ["site456"],
    startDate: ISODate("2024-06-15T00:00:00Z"),
    endDate: ISODate("2024-06-30T23:59:59Z"),
    freeSpinCount: 100,
    isAllPlayers: false,
    playerIds: ["player123", "player456", "player789"],
    games: [
        {
            gameId: 1001,
            gameName: "Classic Fruit",
            gameType: "ClassicSlot"
        }
    ],
    createdAt: ISODate("2024-06-10T10:00:00Z"),
    updatedAt: ISODate("2024-06-10T10:00:00Z")
}

// Example player promotion document
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j4"),
    playerId: "player123",
    operatorId: "op123",
    siteId: "site456",
    promotionId: ObjectId("65f1a2b3c4d5e6f7g8h9i0j3"),
    promotionCode: "vip-classic-free-spins",
    remainingSpins: 100,
    lastUsedDate: null,
    createdAt: ISODate("2024-06-15T00:00:00Z"),
    updatedAt: ISODate("2024-06-15T00:00:00Z")
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