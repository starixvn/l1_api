# MongoDB Schema for Bets

## Collections

### 1. Bets Collection
```javascript
{
    _id: ObjectId,                    // BetId
    seq: String,                      // Sequence number
    playerId: String,                 // Player identifier
    siteId: String,                   // Site identifier
    timestamp: ISODate,               // When the bet was placed
    gameRoundId: Number,              // Game round identifier
    gameTypeId: Number,               // Game type identifier
    choiceId: Number,                 // Choice identifier
    amount: Number,                   // Amount wagered
    currency: String,                 // Currency code
    roundEndTime: ISODate?,           // Optional round end time
    ip: String,                       // Player's IP address
    isFreeSpin: Boolean,              // Whether this is a free spin bet
    contributionAmount: Number,       // Contribution amount
    contributionRate: Number,         // Contribution rate
    isFreeBet: Boolean,               // Whether this is a free bet
    operatorId: String,               // Operator identifier
    userId: String,                   // User identifier
    promotionId: ObjectId?,           // Optional reference to Promotions collection
    promotionCode: String?,           // Optional promotion code for quick reference
    winAmount: Number,                // Amount won
    paid: Boolean,                    // Whether the bet has been paid out
    createdAt: ISODate,
    updatedAt: ISODate
}
```

## Example Data

### 1. Regular Bet (No Promotion)
```javascript
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),
    seq: "BET20240601100000",
    playerId: "player123",
    siteId: "site456",
    timestamp: ISODate("2024-06-01T10:00:00Z"),
    gameRoundId: 10001,
    gameTypeId: 1,
    choiceId: 1,
    amount: 1.00,
    currency: "EUR",
    roundEndTime: ISODate("2024-06-01T10:00:30Z"),
    ip: "192.168.1.1",
    isFreeSpin: false,
    contributionAmount: 0.10,
    contributionRate: 0.1,
    isFreeBet: false,
    operatorId: "op123",
    userId: "user123",
    promotionId: null,
    promotionCode: null,
    winAmount: 2.50,
    paid: true,
    createdAt: ISODate("2024-06-01T10:00:00Z"),
    updatedAt: ISODate("2024-06-01T10:00:01Z")
}
```

### 2. Promotion Bet (Free Spins)
```javascript
{
    _id: ObjectId("65f1a2b3c4d5e6f7g8h9i0j2"),
    seq: "BET20240601110000",
    playerId: "player123",
    siteId: "site456",
    timestamp: ISODate("2024-06-01T11:00:00Z"),
    gameRoundId: 10002,
    gameTypeId: 1,
    choiceId: 1,
    amount: 0.00,                     // Free spin bet amount is 0
    currency: "EUR",
    roundEndTime: ISODate("2024-06-01T11:00:30Z"),
    ip: "192.168.1.1",
    isFreeSpin: true,
    contributionAmount: 0.00,         // No contribution for free spins
    contributionRate: 0.0,            // No contribution rate for free spins
    isFreeBet: false,
    operatorId: "op123",
    userId: "user123",
    promotionId: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),  // Reference to Summer Classic Free Spins
    promotionCode: "summer-classic-free-spins",
    winAmount: 5.00,
    paid: true,
    createdAt: ISODate("2024-06-01T11:00:00Z"),
    updatedAt: ISODate("2024-06-01T11:00:01Z")
}
```

## Example Queries

### 1. Find Player's Bets for a Specific Promotion
```javascript
db.bets.find({
    playerId: "player123",
    operatorId: "op123",
    siteId: "site456",
    promotionId: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),  // Summer Classic Free Spins
    isFreeSpin: true
})
```

### 2. Find All Free Spin Bets
```javascript
db.bets.find({
    playerId: "player123",
    operatorId: "op123",
    siteId: "site456",
    isFreeSpin: true,
    timestamp: { 
        $gte: ISODate("2024-06-01T00:00:00Z"),
        $lte: ISODate("2024-06-30T23:59:59Z")
    }
})
```

### 3. Calculate Total Free Spin Winnings
```javascript
db.bets.aggregate([
    {
        $match: {
            playerId: "player123",
            operatorId: "op123",
            siteId: "site456",
            promotionId: ObjectId("65f1a2b3c4d5e6f7g8h9i0j1"),
            isFreeSpin: true,
            paid: true
        }
    },
    {
        $group: {
            _id: "$promotionId",
            totalWinAmount: { $sum: "$winAmount" },
            totalBets: { $sum: 1 }
        }
    }
])
```

### 4. Find Unpaid Bets
```javascript
db.bets.find({
    playerId: "player123",
    operatorId: "op123",
    siteId: "site456",
    paid: false
})
``` 