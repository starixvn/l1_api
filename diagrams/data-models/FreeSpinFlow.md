# Free Spin Count Deduction Flow

## Flow Diagram

```mermaid
sequenceDiagram
    participant Player
    participant Game
    participant PP as PlayerPromotion
    participant B as Bet
    participant S as System

    Note over PP: Initial State
    Note over PP: remainingSpins: 50

    Player->>Game: Open Game
    Game->>PP: Check available spins
    PP->>Game: Confirm spins available
    
    Player->>Game: Place Free Spin Bet
    Game->>B: Create Bet
    B->>PP: Deduct 1 spin
    Note over PP: remainingSpins: 49
    B->>PP: Record bet with promotionId
    Note over B: isFreeSpin: true
    Note over B: amount: 0.00
    Note over B: winAmount: 5.00

    alt Win
        B->>PP: Update lastUsedDate
        Note over B: paid: true
    else Loss
        B->>PP: Update lastUsedDate
        Note over B: paid: true
    end

    Note over PP: Check Completion
    alt All spins used
        Note over PP: remainingSpins: 0
        Note over PP: Promotion completed
    else Spins remaining
        Note over PP: Continue promotion
    end
```

## State Transitions

### 1. Promotion Creation
```mermaid
stateDiagram-v2
    [*] --> PromotionCreated
    PromotionCreated --> Active: Start date reached
    Active --> Ended: End date reached
```

### 2. Player Promotion States
```mermaid
stateDiagram-v2
    [*] --> Available: Promotion assigned
    Available --> InProgress: First free spin used
    InProgress --> Completed: All spins used
    InProgress --> Available: More spins available
    Completed --> [*]
```

### 3. Free Spin Bet States
```mermaid
stateDiagram-v2
    [*] --> Created: Bet placed
    Created --> Paid: Win/Loss determined
    Paid --> [*]
```

## Data Flow

1. **Player to Game**:
   - Player opens game
   - Game checks available spins
   - Player places free spin bet

2. **Game to Bet**:
   - Bet is created
   - Spin is deducted from PlayerPromotion
   - Bet is recorded with promotion reference

3. **Bet to PlayerPromotion**:
   - Update `lastUsedDate` after spin
   - Check completion when `remainingSpins` reaches 0

## Example Scenarios

### 1. Successful Free Spin
```javascript
// Before spin
PlayerPromotion: { remainingSpins: 50 }
Bet: { isFreeSpin: true, amount: 0.00 }

// After spin
PlayerPromotion: { remainingSpins: 49, lastUsedDate: "2024-06-01T11:00:00Z" }
Bet: { isFreeSpin: true, amount: 0.00, winAmount: 5.00, paid: true }
```

### 2. Last Free Spin
```javascript
// Before spin
PlayerPromotion: { remainingSpins: 1 }
Bet: { isFreeSpin: true, amount: 0.00 }

// After spin
PlayerPromotion: { remainingSpins: 0, lastUsedDate: "2024-06-01T11:00:00Z" }
Bet: { isFreeSpin: true, amount: 0.00, winAmount: 0.00, paid: true }
```

## Validation Rules

1. **Game Opening**:
   - Check if player has available spins
   - Verify game is eligible for promotion
   - Confirm promotion is active

2. **Spin Deduction**:
   - Must have remaining spins > 0
   - Must be within promotion dates
   - Must be for eligible game

3. **Bet Creation**:
   - Verify promotion is active
   - Verify player is eligible
   - Set isFreeSpin = true
   - Set amount = 0.00 