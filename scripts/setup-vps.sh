#!/bin/bash
# =============================================================================
# Kouroukan - Setup VPS (Ubuntu 22.04+)
# =============================================================================
# Usage : sudo bash setup-vps.sh
# Ce script installe Docker, configure le firewall, cree l'utilisateur deploy
# et prepare le serveur pour le deploiement de Kouroukan.
# =============================================================================

set -euo pipefail

echo "============================================="
echo " Kouroukan — Configuration du VPS"
echo "============================================="

# Verifier les privileges root
if [[ $EUID -ne 0 ]]; then
    echo "Ce script doit etre execute en tant que root (sudo)"
    exit 1
fi

# -------------------------------------------------------------------
# 1. Mise a jour du systeme
# -------------------------------------------------------------------
echo ""
echo "--- [1/7] Mise a jour du systeme ---"
apt-get update -y
apt-get upgrade -y
apt-get install -y \
    curl \
    wget \
    git \
    unzip \
    htop \
    nano \
    ca-certificates \
    gnupg \
    lsb-release \
    fail2ban

# -------------------------------------------------------------------
# 2. Installation de Docker
# -------------------------------------------------------------------
echo ""
echo "--- [2/7] Installation de Docker ---"

if ! command -v docker &> /dev/null; then
    # Cle GPG officielle Docker
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
        gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg

    # Depot Docker
    echo \
        "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
        https://download.docker.com/linux/ubuntu \
        $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
        tee /etc/apt/sources.list.d/docker.list > /dev/null

    apt-get update -y
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

    echo "Docker installe avec succes."
else
    echo "Docker deja installe."
fi

systemctl enable docker
systemctl start docker

# -------------------------------------------------------------------
# 3. Creer l'utilisateur deploy
# -------------------------------------------------------------------
echo ""
echo "--- [3/7] Creation de l'utilisateur deploy ---"

DEPLOY_USER="deploy"

if ! id "$DEPLOY_USER" &>/dev/null; then
    adduser --disabled-password --gecos "" "$DEPLOY_USER"
    usermod -aG docker "$DEPLOY_USER"
    usermod -aG sudo "$DEPLOY_USER"

    # Configurer les cles SSH
    mkdir -p /home/$DEPLOY_USER/.ssh
    chmod 700 /home/$DEPLOY_USER/.ssh

    # Copier les cles SSH du root si elles existent
    if [ -f /root/.ssh/authorized_keys ]; then
        cp /root/.ssh/authorized_keys /home/$DEPLOY_USER/.ssh/authorized_keys
    else
        touch /home/$DEPLOY_USER/.ssh/authorized_keys
    fi

    chmod 600 /home/$DEPLOY_USER/.ssh/authorized_keys
    chown -R $DEPLOY_USER:$DEPLOY_USER /home/$DEPLOY_USER/.ssh

    echo "Utilisateur '$DEPLOY_USER' cree et ajoute au groupe docker."
else
    echo "Utilisateur '$DEPLOY_USER' existe deja."
fi

# -------------------------------------------------------------------
# 4. Configuration du firewall (UFW)
# -------------------------------------------------------------------
echo ""
echo "--- [4/7] Configuration du firewall UFW ---"

apt-get install -y ufw

ufw default deny incoming
ufw default allow outgoing

# SSH
ufw allow 22/tcp comment "SSH"

# HTTP / HTTPS
ufw allow 80/tcp comment "HTTP"
ufw allow 443/tcp comment "HTTPS"

# Activer UFW (non-interactif)
echo "y" | ufw enable

ufw status verbose

# -------------------------------------------------------------------
# 5. Installation de Certbot (Let's Encrypt)
# -------------------------------------------------------------------
echo ""
echo "--- [5/7] Installation de Certbot ---"

apt-get install -y certbot

echo "Certbot installe. Pour generer les certificats :"
echo "  certbot certonly --webroot -w /var/www/certbot \\"
echo "    -d www.kouroukan.gn -d app.kouroukan.gn \\"
echo "    --email contact@kouroukan.gn --agree-tos --no-eff-email"

# -------------------------------------------------------------------
# 6. Preparer le repertoire de deploiement
# -------------------------------------------------------------------
echo ""
echo "--- [6/7] Preparation du repertoire de deploiement ---"

DEPLOY_DIR="/opt/kouroukan"

mkdir -p "$DEPLOY_DIR"
chown -R $DEPLOY_USER:$DEPLOY_USER "$DEPLOY_DIR"

echo "Repertoire $DEPLOY_DIR cree."

# -------------------------------------------------------------------
# 7. Configuration de fail2ban
# -------------------------------------------------------------------
echo ""
echo "--- [7/7] Configuration de fail2ban ---"

cat > /etc/fail2ban/jail.local << 'JAIL'
[DEFAULT]
bantime  = 3600
findtime = 600
maxretry = 5

[sshd]
enabled = true
port    = ssh
filter  = sshd
logpath = /var/log/auth.log
maxretry = 3
JAIL

systemctl enable fail2ban
systemctl restart fail2ban

# -------------------------------------------------------------------
# Resume
# -------------------------------------------------------------------
echo ""
echo "============================================="
echo " Configuration terminee !"
echo "============================================="
echo ""
echo " Prochaines etapes :"
echo "   1. Ajouter votre cle SSH publique dans :"
echo "      /home/$DEPLOY_USER/.ssh/authorized_keys"
echo ""
echo "   2. Cloner le depot :"
echo "      su - $DEPLOY_USER"
echo "      cd /opt/kouroukan"
echo "      git clone <REPO_URL> ."
echo ""
echo "   3. Configurer les variables d'environnement :"
echo "      cd docker"
echo "      cp .env.example .env"
echo "      nano .env"
echo ""
echo "   4. Generer les certificats SSL :"
echo "      sudo certbot certonly --webroot -w /var/www/certbot \\"
echo "        -d www.kouroukan.gn -d app.kouroukan.gn"
echo ""
echo "   5. Lancer l'application :"
echo "      docker compose build --parallel"
echo "      docker compose up -d"
echo ""
echo "   6. Initialiser Ollama :"
echo "      bash ../scripts/setup-ollama.sh"
echo ""
echo "   7. Configurer le backup quotidien :"
echo "      crontab -e"
echo "      0 2 * * * /opt/kouroukan/scripts/backup.sh"
echo ""
echo "============================================="
