# NATS JetStream Migration Guide

## Overview

This guide covers migrating existing NATS JetStream streams to support enhanced ack/nak patterns with improved configuration.

## ⚠️ **Important Notes**

- **New deployments**: Automatically get enhanced stream configuration
- **Existing streams**: Require manual migration (this guide)
- **Zero downtime**: Migration can be done without service interruption

## 🔄 **Migration Steps**

### **Step 1: Check Current Stream Configuration**

Use NATS CLI to inspect your existing stream:

```bash
# List all streams
nats stream list

# Get detailed stream info
nats stream info <stream_name>
```

### **Step 2: Update Application Configuration**

Update your `appsettings.json` with enhanced properties:

```json
{
  "StreamSettings": {
    "Host": "nats://your-nats-server:4222",
    "StreamName": "api_events",
    "Postfix": "your_postfix",
    "SubjectPrefix": "your.prefix.api",
    "Enable": true,
    "PublishTimeoutMs": 500,
    
    // Enhanced configuration for ack/nak support
    "StorageType": "File",           // File or Memory
    "RetentionPolicy": "Limits",     // Limits, Interest, WorkQueue  
    "DiscardPolicy": "Old",          // Old, New
    "MaxAgeHours": 24,              // Increased from 1 hour
    "MaxMessages": 1000000,         // Increased from 100K
    "MaxBytes": 1073741824,         // 1GB limit
    "Replicas": 1,
    "DuplicateWindowMs": 120000,    // 2 minutes
    "NoAck": false,                 // Enable ack/nak
    "AllowRollup": false,
    "DenyDelete": false,
    "DenyPurge": false
  }
}
```

### **Step 3: Create New Stream with Enhanced Configuration**

#### **Option A: Create with Different Name (Recommended)**

```bash
# Create new stream with enhanced config
nats stream add \
  --name="api_events_your_postfix_v2" \
  --subjects="your.prefix.api.>" \
  --storage=file \
  --retention=limits \
  --discard=old \
  --max-age=24h \
  --max-messages=1000000 \
  --max-bytes=1073741824 \
  --replicas=1 \
  --duplicate-window=2m
```

#### **Option B: Update Existing Stream (Limited)**

Some properties can be updated in-place:

```bash
# Update limits (safe operations)
nats stream edit api_events_your_postfix \
  --max-age=24h \
  --max-messages=1000000 \
  --max-bytes=1073741824
```

**⚠️ Cannot change**: StorageType, StreamName, Subjects, Replicas

### **Step 4: Migrate Data (if needed)**

If you created a new stream, you can:

1. **Parallel publishing**: Publish to both streams temporarily
2. **Consumer migration**: Move consumers to new stream gradually  
3. **Data replay**: Use NATS sourcing/mirroring features

```bash
# Example: Create a source to replay data
nats stream add \
  --name="migration_replay" \
  --source="api_events_your_postfix" \
  --storage=memory
```

### **Step 5: Update Application Configuration**

Update your app configuration to use the new stream name:

```json
{
  "StreamSettings": {
    "StreamName": "api_events",
    "Postfix": "your_postfix_v2"  // Point to new stream
  }
}
```

### **Step 6: Create Durable Consumers**

For ack/nak pattern, create durable consumers:

```bash
# Create durable consumer with explicit ack
nats consumer add \
  --stream="api_events_your_postfix_v2" \
  --consumer="api_consumer_v2" \
  --deliver=all \
  --ack=explicit \
  --max-deliver=10 \
  --ack-wait=30s \
  --max-ack-pending=1000 \
  --replay=instant \
  --durable
```

### **Step 7: Test and Verify**

```bash
# Test publishing
nats pub your.prefix.api.test "test message"

# Test consuming with ack
nats consumer next api_events_your_postfix_v2 api_consumer_v2

# Monitor stream health
nats stream info api_events_your_postfix_v2
```

### **Step 8: Cleanup (After Verification)**

Once migration is complete and verified:

```bash
# Remove old stream (⚠️ IRREVERSIBLE)
nats stream delete api_events_your_postfix
```

## 📋 **Environment-Specific Migration**

### **Development/Local**
- Safe to delete and recreate streams
- Update `appsettings.LOCAL.json`

### **Staging/UAT**  
- Test migration process
- Verify consumer behavior
- Update `appsettings.SIT.json` or `appsettings.UAT.json`

### **Production**
- Plan maintenance window
- Use parallel streams approach
- Monitor closely
- Update `appsettings.PROD.json`

## 🛠️ **Troubleshooting**

### **"Stream already exists" Error**
```bash
# Check existing configuration
nats stream info <stream_name>

# Update manually using NATS CLI
nats stream edit <stream_name>
```

### **Configuration Mismatch**
- Check Sentry logs for specific configuration differences
- Use CLI to inspect current vs desired configuration
- Update manually where possible

### **Consumer Issues**
```bash
# List consumers
nats consumer list <stream_name>

# Check consumer status
nats consumer info <stream_name> <consumer_name>
```

## ✅ **Best Practices**

1. **Test in development first**
2. **Use versioned stream names** (api_events_v2)
3. **Monitor stream metrics** during migration
4. **Keep backups** of important data
5. **Update configs gradually** across environments
6. **Document custom configurations** for your environment

## 📞 **Support**

If you encounter issues during migration:
1. Check NATS server logs
2. Review Sentry alerts
3. Use NATS CLI for troubleshooting
4. Test with simple pub/sub first 