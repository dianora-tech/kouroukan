# Kouroukan

Plateforme de gestion d'etablissement scolaire construite en architecture microservices.

## Architecture

```
                         +-----------+
                         |   Nginx   |  :80 / :443
                         +-----+-----+
                               |
                 +-------------+-------------+
                 |                           |
          +------+------+            +------+------+
          |  Frontend   |            |   Portal    |
          |  Nuxt 3 SSR |  :3000    |  Nuxt 3 SSG |  :8080
          +------+------+            +-------------+
                 |
          +------+------+
          |   Gateway   |  :5000
          |  .NET 8     |
          |  YARP Proxy |
          +------+------+
                 |
    +------------+------------+------------+--- ...
    |            |            |            |
 :5001        :5002        :5003        :5004
Inscriptions Pedagogie  Evaluations  Presences
    |            |            |            |
    +------------+------------+------------+
                 |
     +-----------+-----------+-----------+
     |           |           |           |
  PostgreSQL   Redis     RabbitMQ     MinIO
   :5432       :6379      :5672       :9000
```

## Stack technique

| Couche | Technologie |
|--------|------------|
| Frontend | Nuxt 3.16 (SSR), Vue 3.5, Pinia 2.3, Nuxt UI 3, i18n |
| Portal | Nuxt 3.16 (SSG), Nuxt Content 3, Nuxt SEO |
| Gateway | .NET 8, YARP Reverse Proxy, JWT (HS512), Swagger |
| Microservices | .NET 8, Dapper, Clean Architecture |
| Base de donnees | PostgreSQL 16, 11 schemas, 22 migrations |
| Cache | Redis 7 (LRU, 256 MB) + .NET cache service (Quartz.NET) |
| Messaging | RabbitMQ 3 (communication inter-services) |
| Stockage | MinIO (compatible S3) |
| IA | Ollama (Mistral) pour le support intelligent |
| Monitoring | Uptime Kuma |
| CI/CD | GitHub Actions (path filters) + deploy SSH vers VPS |

## Services backend

| Service | Port | Description |
|---------|------|-------------|
| Gateway | 5000 | API Gateway, authentification JWT, reverse proxy YARP |
| Inscriptions | 5001 | Gestion des eleves, parents, dossiers |
| Pedagogie | 5002 | Classes, matieres, emplois du temps, programmes |
| Evaluations | 5003 | Notes, bulletins, moyennes, classements |
| Presences | 5004 | Appels, absences, retards, justificatifs |
| Finances | 5005 | Frais de scolarite, paiements, comptabilite |
| Personnel | 5006 | Enseignants, administratifs, contrats |
| Communication | 5007 | Messages, notifications, annonces |
| BDE | 5008 | Bureau des eleves, associations, evenements |
| Documents | 5009 | GED, modeles, generation de documents |
| Services Premium | 5010 | Modules supplementaires payants |
| Support | 5011 | Tickets, FAQ, assistant IA (Ollama) |
| Cache | 5100 | Service cache centralise Redis L2 |

## Infrastructure Docker

| Service | Port | Usage |
|---------|------|-------|
| PostgreSQL 16 | 5432 | Base de donnees principale |
| Redis 7 | 6379 | Cache distribue |
| RabbitMQ 3 | 5672 / 15672 | Bus de messages / Console admin |
| MinIO | 9000 / 9001 | Stockage objet / Console admin |
| Ollama | 11434 | Service IA generative |
| Uptime Kuma | 3001 | Monitoring des services |
| Nginx | 80 / 443 | Reverse proxy + SSL (production) |

## Structure du projet

```
kouroukan/
├── .github/workflows/       # CI/CD (ci.yml + deploy.yml)
├── docker/                  # Docker Compose, Nginx, .env
├── sql/                     # 22 migrations PostgreSQL (V001 a V022)
├── scripts/                 # Scripts utilitaires
├── kouroukan.front/         # Frontend Nuxt 3 SSR
├── kouroukan.portal/        # Portal public Nuxt 3 SSG
└── vs2022/                  # Backend .NET 8
    ├── kouroukan.api/       #   Gateway API
    ├── gn-core-libraries/   #   Libs partagees (Dapper, Security, Messaging, Validation)
    ├── gn-cache/            #   Service cache
    ├── inscriptions.service/
    ├── pedagogie.service/
    ├── evaluations.service/
    ├── presences.service/
    ├── finances.service/
    ├── personnel.service/
    ├── communication.service/
    ├── bde.service/
    ├── documents.service/
    ├── services-premium.service/
    └── support.service/
```

## Demarrage rapide

### Prerequis

- Docker & Docker Compose
- Node.js 20+
- .NET 8 SDK

### Lancement

```bash
# 1. Cloner le repo
git clone https://github.com/Iboubai/kouroukan.git
cd kouroukan

# 2. Configurer les variables d'environnement
cp docker/.env.example docker/.env
# Editer docker/.env avec vos valeurs (mots de passe, JWT secret, etc.)

# 3. Lancer tous les services
cd docker
docker compose up -d --build

# 4. Executer les migrations SQL
# Les migrations sont dans sql/V001 a V022 et s'executent automatiquement
# ou manuellement :
for f in ../sql/V*.sql; do
  docker compose exec -T postgres psql -U kouroukan -d kouroukan -f /dev/stdin < "$f"
done

# 5. Acceder a l'application
# Frontend  : http://localhost:3000
# Portal    : http://localhost:8080
# Gateway   : http://localhost:5000/swagger
# RabbitMQ  : http://localhost:15672
# MinIO     : http://localhost:9001
# Monitoring: http://localhost:3001
```

### Developpement frontend

```bash
cd kouroukan.front
npm ci --legacy-peer-deps
npm run dev
# -> http://localhost:3000
```

### Developpement backend

```bash
cd vs2022/kouroukan.api
dotnet restore kouroukan.api.sln
dotnet run --project src/kouroukan.api.Gateway
# -> http://localhost:5000/swagger
```

## Base de donnees

### Schemas PostgreSQL

| Schema | Description |
|--------|-------------|
| `auth` | Utilisateurs, roles, permissions, tokens, CGU |
| `geo` | Regions, prefectures, communes |
| `inscriptions` | Eleves, parents, dossiers, documents |
| `pedagogie` | Classes, matieres, emplois du temps |
| `evaluations` | Notes, bulletins, periodes |
| `presences` | Appels, absences, justificatifs |
| `finances` | Frais, paiements, echeanciers |
| `personnel` | Enseignants, contrats, affectations |
| `communication` | Messages, notifications, modeles |
| `bde` | Associations, evenements, elections |
| `documents` | GED, modeles, generation |
| `services` | Services premium, abonnements |
| `support` | Tickets, FAQ, base de connaissances |

### Migrations

22 fichiers SQL versiones (`V001` a `V022`) couvrant :
- Creation des schemas et tables
- Contraintes inter-modules
- Seed data (roles, permissions, CGU, donnees de reference)
- Fonctions, vues, recherche full-text, index

## CI/CD

### Pipeline CI (GitHub Actions)

Le CI utilise des **path filters** pour n'executer que les tests pertinents :

| Modification | Jobs lances |
|-------------|-------------|
| `kouroukan.front/**` | Lint + Tests Frontend |
| `kouroukan.portal/**` | Lint Portal |
| `vs2022/kouroukan.api/**` | Tests Gateway |
| `vs2022/*.service/**` | Tests Microservices |
| `vs2022/gn-core-libraries/**` | Tests Gateway + tous les services |
| `sql/**` | Validation des migrations |
| `docker/**` | Build Docker |

### Pipeline CD

Deploiement automatique sur VPS via SSH apres un push sur `main` et un CI reussi :
1. `git pull` sur le serveur
2. `docker compose build --parallel`
3. `docker compose up -d`
4. Health checks (PostgreSQL, Redis, RabbitMQ, Gateway, Frontend)
5. Notification Telegram

## Securite

- Authentification JWT (HS512) avec access + refresh tokens
- Hashage des mots de passe avec Argon2id
- RBAC : 13 roles, 48 permissions granulaires
- Protection CSRF en production
- Guard middleware sur toutes les routes frontend
- Validation des entrees (GnValidation)
- Protection SQL injection (GnDapper)

## Tests

```bash
# Frontend - tests unitaires
cd kouroukan.front && npm run test

# Frontend - tests E2E
cd kouroukan.front && npm run test:e2e

# Backend - tests unitaires + integration (par service)
cd vs2022/inscriptions.service && dotnet test

# Backend - tous les services
for sln in vs2022/*/*.sln; do dotnet test "$sln"; done
```

## Licence

Projet open-source.
