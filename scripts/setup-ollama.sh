#!/bin/bash
# =============================================================================
# Kouroukan - Initialisation Ollama
# =============================================================================
# Usage : bash setup-ollama.sh
#
# Ce script attend le demarrage d'Ollama puis telecharge le modele IA
# requis pour le module support (aide generative).
#
# Modeles disponibles :
#   - mistral  (7B params, ~4 Go RAM) — recommande
#   - phi      (3B params, ~2 Go RAM) — si RAM limitee
#   - llama3.1 (8B params, ~5 Go RAM) — alternative performante
# =============================================================================

set -euo pipefail

OLLAMA_URL="${OLLAMA_URL:-http://localhost:11434}"
MODEL="${OLLAMA_MODEL:-mistral}"

echo "============================================="
echo " Kouroukan — Initialisation Ollama"
echo "============================================="
echo " URL    : $OLLAMA_URL"
echo " Modele : $MODEL"
echo "============================================="

# -------------------------------------------------------------------
# 1. Attendre le demarrage d'Ollama
# -------------------------------------------------------------------
echo ""
echo "Attente du demarrage d'Ollama..."

MAX_RETRIES=30
RETRY_COUNT=0

until curl -sf "$OLLAMA_URL/api/tags" > /dev/null 2>&1; do
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
        echo "ERREUR : Ollama n'a pas demarre apres $MAX_RETRIES tentatives."
        echo "Verifier que le conteneur ollama est en cours d'execution :"
        echo "  docker compose -f docker/docker-compose.yml logs ollama"
        exit 1
    fi
    echo "  Tentative $RETRY_COUNT/$MAX_RETRIES — attente 5s..."
    sleep 5
done

echo "Ollama est operationnel."

# -------------------------------------------------------------------
# 2. Verifier si le modele est deja telecharge
# -------------------------------------------------------------------
echo ""
echo "Verification du modele '$MODEL'..."

MODELS=$(curl -sf "$OLLAMA_URL/api/tags" | grep -o "\"$MODEL" || true)

if [ -n "$MODELS" ]; then
    echo "Le modele '$MODEL' est deja disponible."
else
    # -------------------------------------------------------------------
    # 3. Telecharger le modele
    # -------------------------------------------------------------------
    echo "Telechargement du modele '$MODEL'..."
    echo "(Cela peut prendre plusieurs minutes selon la connexion)"
    echo ""

    curl -X POST "$OLLAMA_URL/api/pull" \
        -H "Content-Type: application/json" \
        -d "{\"name\": \"$MODEL\"}" \
        --no-buffer

    echo ""
    echo "Telechargement termine."
fi

# -------------------------------------------------------------------
# 4. Verification finale
# -------------------------------------------------------------------
echo ""
echo "Verification finale..."

RESPONSE=$(curl -sf "$OLLAMA_URL/api/tags")
echo "Modeles disponibles :"
echo "$RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$RESPONSE"

echo ""
echo "============================================="
echo " Ollama est pret."
echo " Le module support peut utiliser l'aide IA."
echo "============================================="
