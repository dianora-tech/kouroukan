-- =============================================================================
-- Kouroukan - Initialisation des schemas PostgreSQL
-- =============================================================================
-- Ce script est execute automatiquement au premier demarrage du conteneur
-- PostgreSQL via le mecanisme /docker-entrypoint-initdb.d/
-- =============================================================================

-- Schemas par module
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS inscriptions;
CREATE SCHEMA IF NOT EXISTS pedagogie;
CREATE SCHEMA IF NOT EXISTS evaluations;
CREATE SCHEMA IF NOT EXISTS presences;
CREATE SCHEMA IF NOT EXISTS finances;
CREATE SCHEMA IF NOT EXISTS personnel;
CREATE SCHEMA IF NOT EXISTS communication;
CREATE SCHEMA IF NOT EXISTS bde;
CREATE SCHEMA IF NOT EXISTS documents;
CREATE SCHEMA IF NOT EXISTS services;
CREATE SCHEMA IF NOT EXISTS support;
CREATE SCHEMA IF NOT EXISTS geo;
CREATE SCHEMA IF NOT EXISTS audit;

-- Extensions utiles
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
