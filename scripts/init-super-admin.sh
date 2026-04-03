#!/usr/bin/env bash
# =============================================================================
# init-super-admin.sh
# =============================================================================
# Initialises the super admin password in the kouroukan database.
#
# This script is IDEMPOTENT: it only updates the user whose password_hash is
# still set to the migration placeholder 'CHANGE_ME_AT_DEPLOY'.  Running it
# against a database where the password has already been set is a safe no-op.
#
# Environment variables (with defaults):
#   POSTGRES_HOST     — default: localhost
#   POSTGRES_PORT     — default: 5432
#   POSTGRES_DB       — default: kouroukan
#   POSTGRES_USER     — default: kouroukan
#   POSTGRES_PASSWORD — default: kouroukan
#   ADMIN_PASSWORD    — REQUIRED, no default
#
# Requirements:
#   - psql (PostgreSQL client)
#   - python3 with argon2-cffi  (pip install argon2-cffi)
# =============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Defaults
# ---------------------------------------------------------------------------
POSTGRES_HOST="${POSTGRES_HOST:-localhost}"
POSTGRES_PORT="${POSTGRES_PORT:-5432}"
POSTGRES_DB="${POSTGRES_DB:-kouroukan}"
POSTGRES_USER="${POSTGRES_USER:-kouroukan}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-kouroukan}"
export PGPASSWORD="${POSTGRES_PASSWORD}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# ---------------------------------------------------------------------------
# Validation
# ---------------------------------------------------------------------------
if [ -z "${ADMIN_PASSWORD:-}" ]; then
    echo "ERROR: ADMIN_PASSWORD environment variable is required." >&2
    exit 1
fi

if [ ${#ADMIN_PASSWORD} -lt 12 ]; then
    echo "ERROR: ADMIN_PASSWORD must be at least 12 characters long." >&2
    exit 1
fi

# Check dependencies
if ! command -v psql &>/dev/null; then
    echo "ERROR: psql is not installed or not in PATH." >&2
    exit 1
fi

if ! command -v python3 &>/dev/null; then
    echo "ERROR: python3 is not installed or not in PATH." >&2
    exit 1
fi

# ---------------------------------------------------------------------------
# Step 1: Check if there is a user with the placeholder password
# ---------------------------------------------------------------------------
echo "Checking for super admin with placeholder password..."

PLACEHOLDER_COUNT=$(psql -h "${POSTGRES_HOST}" -p "${POSTGRES_PORT}" \
    -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -tAc \
    "SELECT COUNT(*) FROM auth.users WHERE password_hash = 'CHANGE_ME_AT_DEPLOY';")

if [ "${PLACEHOLDER_COUNT}" -eq 0 ]; then
    echo "OK: No user with placeholder password found. Nothing to do."
    exit 0
fi

echo "Found ${PLACEHOLDER_COUNT} user(s) with placeholder password."

# ---------------------------------------------------------------------------
# Step 2: Generate the Argon2id hash via Python
# ---------------------------------------------------------------------------
echo "Generating Argon2id hash..."

HASHED_PASSWORD=$(ADMIN_PASSWORD="${ADMIN_PASSWORD}" python3 "${SCRIPT_DIR}/init-super-admin.py")

if [ -z "${HASHED_PASSWORD}" ]; then
    echo "ERROR: Failed to generate password hash." >&2
    exit 1
fi

# Quick sanity check: hash must contain exactly one colon (salt:hash format)
if [[ "${HASHED_PASSWORD}" != *:* ]]; then
    echo "ERROR: Generated hash has unexpected format (expected salt_b64:hash_b64)." >&2
    exit 1
fi

# ---------------------------------------------------------------------------
# Step 3: Update the database (only rows with the placeholder)
# ---------------------------------------------------------------------------
echo "Updating super admin password..."

UPDATED_COUNT=$(psql -h "${POSTGRES_HOST}" -p "${POSTGRES_PORT}" \
    -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -tAc \
    "UPDATE auth.users
        SET password_hash = '${HASHED_PASSWORD}',
            updated_at    = NOW(),
            updated_by    = 'init-super-admin'
      WHERE password_hash = 'CHANGE_ME_AT_DEPLOY'
  RETURNING id;" | wc -l | tr -d ' ')

if [ "${UPDATED_COUNT}" -gt 0 ]; then
    echo "SUCCESS: Updated ${UPDATED_COUNT} user(s) with the new password."
else
    echo "WARNING: UPDATE ran but no rows were affected." >&2
    exit 1
fi

# ---------------------------------------------------------------------------
# Cleanup
# ---------------------------------------------------------------------------
unset PGPASSWORD
echo "Done."
