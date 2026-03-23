-- V026: Ajouter avatar_url pour la photo de profil utilisateur
ALTER TABLE auth.users
    ADD COLUMN IF NOT EXISTS avatar_url VARCHAR(500) NULL;
