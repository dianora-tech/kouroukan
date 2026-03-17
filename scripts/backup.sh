#!/bin/bash
# =============================================================================
# Kouroukan - Backup automatise
# =============================================================================
# Usage : bash backup.sh
# Cron  : 0 2 * * * /opt/kouroukan/scripts/backup.sh >> /var/log/kouroukan-backup.log 2>&1
#
# Effectue un backup de :
#   - PostgreSQL (pg_dump format custom)
#   - Redis (BGSAVE)
#   - MinIO (volumes Docker)
# Rotation : conservation 7 jours
# =============================================================================

set -euo pipefail

# -------------------------------------------------------------------
# Configuration
# -------------------------------------------------------------------
BACKUP_DIR="/opt/kouroukan/backups"
RETENTION_DAYS=7
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
COMPOSE_DIR="/opt/kouroukan/docker"

# Charger les variables d'environnement
if [ -f "$COMPOSE_DIR/.env" ]; then
    set -a
    source "$COMPOSE_DIR/.env"
    set +a
fi

DB_USER="${DB_USER:-kouroukan}"
DB_NAME="${DB_NAME:-kouroukan}"

echo "============================================="
echo " Kouroukan — Backup $TIMESTAMP"
echo "============================================="

# Creer le repertoire de backup
mkdir -p "$BACKUP_DIR"

# -------------------------------------------------------------------
# 1. Backup PostgreSQL
# -------------------------------------------------------------------
echo ""
echo "--- [1/3] Backup PostgreSQL ---"

PG_BACKUP_FILE="$BACKUP_DIR/pg_${DB_NAME}_${TIMESTAMP}.dump"

docker compose -f "$COMPOSE_DIR/docker-compose.yml" exec -T postgres \
    pg_dump -U "$DB_USER" -d "$DB_NAME" -F c -Z 6 \
    > "$PG_BACKUP_FILE"

PG_SIZE=$(du -sh "$PG_BACKUP_FILE" | cut -f1)
echo "PostgreSQL backup : $PG_BACKUP_FILE ($PG_SIZE)"

# -------------------------------------------------------------------
# 2. Backup Redis
# -------------------------------------------------------------------
echo ""
echo "--- [2/3] Backup Redis ---"

docker compose -f "$COMPOSE_DIR/docker-compose.yml" exec -T redis \
    redis-cli BGSAVE

# Attendre la fin du BGSAVE
sleep 5

REDIS_BACKUP_FILE="$BACKUP_DIR/redis_${TIMESTAMP}.rdb"

docker compose -f "$COMPOSE_DIR/docker-compose.yml" cp \
    redis:/data/dump.rdb "$REDIS_BACKUP_FILE"

REDIS_SIZE=$(du -sh "$REDIS_BACKUP_FILE" | cut -f1)
echo "Redis backup : $REDIS_BACKUP_FILE ($REDIS_SIZE)"

# -------------------------------------------------------------------
# 3. Backup MinIO (donnees stockees)
# -------------------------------------------------------------------
echo ""
echo "--- [3/3] Backup MinIO ---"

MINIO_BACKUP_FILE="$BACKUP_DIR/minio_${TIMESTAMP}.tar.gz"

docker compose -f "$COMPOSE_DIR/docker-compose.yml" exec -T minio \
    tar czf - /data 2>/dev/null \
    > "$MINIO_BACKUP_FILE"

MINIO_SIZE=$(du -sh "$MINIO_BACKUP_FILE" | cut -f1)
echo "MinIO backup : $MINIO_BACKUP_FILE ($MINIO_SIZE)"

# -------------------------------------------------------------------
# 4. Rotation des anciens backups
# -------------------------------------------------------------------
echo ""
echo "--- Rotation (conservation $RETENTION_DAYS jours) ---"

DELETED_COUNT=$(find "$BACKUP_DIR" -name "*.dump" -o -name "*.rdb" -o -name "*.tar.gz" \
    -mtime +$RETENTION_DAYS -print -delete 2>/dev/null | wc -l)

echo "$DELETED_COUNT ancien(s) backup(s) supprime(s)."

# -------------------------------------------------------------------
# Resume
# -------------------------------------------------------------------
echo ""
echo "============================================="
echo " Backup termine avec succes"
echo "============================================="
echo " PostgreSQL : $PG_SIZE"
echo " Redis      : $REDIS_SIZE"
echo " MinIO      : $MINIO_SIZE"
echo " Emplacement: $BACKUP_DIR"
echo "============================================="
