#!/bin/bash
# =============================================================================
# Kouroukan - Build Docker par lots (adapté aux serveurs à faible RAM)
# =============================================================================
#
# Sur un serveur avec peu de RAM (< 8 Go), le build parallèle de tous les
# services .NET échoue par manque de mémoire. Ce script build les images
# par lots de BATCH_SIZE services simultanés, avec nettoyage entre chaque lot.
#
# Usage:
#   ./scripts/build-batched.sh                    # Build tout par lots de 2
#   BATCH_SIZE=3 ./scripts/build-batched.sh       # Build par lots de 3
#   ./scripts/build-batched.sh gateway frontend   # Build uniquement ces services
#
# Pré-requis:
#   - Docker avec BuildKit
#   - Être à la racine du projet (ou passer PROJECT_DIR)
#
# =============================================================================

set -euo pipefail

# Configuration
BATCH_SIZE="${BATCH_SIZE:-2}"
PROJECT_DIR="${PROJECT_DIR:-$(cd "$(dirname "$0")/.." && pwd)}"
APP_VERSION="${APP_VERSION:-v1.0-dev}"
COMPOSE_FILE="${COMPOSE_FILE:-docker-compose.coolify.yml}"

# Couleurs
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

cd "$PROJECT_DIR"

# ---------------------------------------------------------------------------
# Définition des services et leurs groupes de priorité
# ---------------------------------------------------------------------------
# Groupe 1 : Infrastructure (images pré-buildées, pas de build nécessaire)
# Groupe 2 : Frontend + Portal (Node.js - build rapide)
# Groupe 3 : Gateway + Cache (.NET - build moyen)
# Groupe 4 : Microservices .NET (build lourd, par lots)
# ---------------------------------------------------------------------------

FRONTEND_SERVICES=(
  "portal"
  "frontend"
)

CORE_SERVICES=(
  "gateway"
  "cache"
)

MICROSERVICES=(
  "inscriptions-service"
  "pedagogie-service"
  "evaluations-service"
  "presences-service"
  "finances-service"
  "personnel-service"
  "communication-service"
  "bde-service"
  "documents-service"
  "services-premium-service"
  "support-service"
)

# ---------------------------------------------------------------------------
# Fonctions
# ---------------------------------------------------------------------------

log_info()  { echo -e "${BLUE}[INFO]${NC}  $*"; }
log_ok()    { echo -e "${GREEN}[OK]${NC}    $*"; }
log_warn()  { echo -e "${YELLOW}[WARN]${NC}  $*"; }
log_error() { echo -e "${RED}[ERROR]${NC} $*"; }

build_service() {
  local service="$1"
  local log_file="/tmp/build-${service}.log"

  docker compose -f "$COMPOSE_FILE" build --no-cache "$service" > "$log_file" 2>&1
  return $?
}

build_batch() {
  local -n services_ref=$1
  local group_name="$2"
  local total=${#services_ref[@]}

  if [ "$total" -eq 0 ]; then
    return 0
  fi

  echo ""
  echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
  echo -e "${BLUE}  $group_name ($total services, lots de $BATCH_SIZE)${NC}"
  echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

  local built=0
  local failed=0
  local failed_services=()

  for ((i=0; i<total; i+=BATCH_SIZE)); do
    local batch_num=$((i/BATCH_SIZE + 1))
    local batch_end=$((i+BATCH_SIZE < total ? i+BATCH_SIZE : total))

    echo ""
    log_info "Lot $batch_num — services $((i+1)) à $batch_end / $total"

    # Lancer les builds en parallèle dans ce lot
    local pids=()
    local batch_services=()
    for ((j=i; j<batch_end; j++)); do
      local svc="${services_ref[$j]}"
      batch_services+=("$svc")
      log_info "  Building $svc..."
      build_service "$svc" &
      pids+=($!)
    done

    # Attendre la fin de chaque build du lot
    for ((j=0; j<${#pids[@]}; j++)); do
      local svc="${batch_services[$j]}"
      if wait "${pids[$j]}"; then
        log_ok "  $svc"
        built=$((built+1))
      else
        log_error "  $svc FAILED (voir /tmp/build-${svc}.log)"
        failed=$((failed+1))
        failed_services+=("$svc")
      fi
    done

    log_info "Progression: $built/$total buildés"

    # Nettoyage entre les lots pour libérer de la mémoire
    if [ "$batch_end" -lt "$total" ]; then
      docker builder prune -f > /dev/null 2>&1 || true
    fi
  done

  if [ "$failed" -gt 0 ]; then
    log_error "$failed service(s) en échec : ${failed_services[*]}"
    return 1
  fi

  return 0
}

# ---------------------------------------------------------------------------
# Mode sélectif : si des services sont passés en argument
# ---------------------------------------------------------------------------
if [ $# -gt 0 ]; then
  SELECTED=("$@")
  log_info "Build sélectif de ${#SELECTED[@]} service(s): ${SELECTED[*]}"
  build_batch SELECTED "Build sélectif"
  exit $?
fi

# ---------------------------------------------------------------------------
# Build complet par groupes
# ---------------------------------------------------------------------------

echo ""
echo -e "${BLUE}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  Kouroukan — Build par lots (batch_size=$BATCH_SIZE)              ║${NC}"
echo -e "${BLUE}║  RAM disponible: $(free -h 2>/dev/null | awk '/Mem:/{print $7}' || echo 'N/A')                                   ║${NC}"
echo -e "${BLUE}╚═══════════════════════════════════════════════════════════╝${NC}"

START_TIME=$(date +%s)

# Groupe 2 : Frontend (Node.js)
build_batch FRONTEND_SERVICES "Groupe 1 — Frontend (Node.js)"

# Groupe 3 : Core .NET
build_batch CORE_SERVICES "Groupe 2 — Core (Gateway + Cache)"

# Nettoyage avant les microservices
docker builder prune -f > /dev/null 2>&1 || true

# Groupe 4 : Microservices .NET (les plus lourds)
build_batch MICROSERVICES "Groupe 3 — Microservices .NET"

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))
MINUTES=$((DURATION / 60))
SECONDS=$((DURATION % 60))

echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  BUILD TERMINÉ en ${MINUTES}m${SECONDS}s                              ║${NC}"
echo -e "${GREEN}║  15/15 services buildés avec succès                      ║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"

# Nettoyage final
docker builder prune -f > /dev/null 2>&1 || true
