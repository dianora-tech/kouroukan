-- ============================================================================
-- V001__create_schemas.sql
-- Kouroukan - Creation des schemas PostgreSQL
-- ============================================================================

-- Schemas fondation
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS geo;

-- Schemas modules metier
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
