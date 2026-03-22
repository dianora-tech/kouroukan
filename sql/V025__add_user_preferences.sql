-- V025: Table de preferences utilisateur (extensible)
CREATE TABLE IF NOT EXISTS auth.user_preferences (
    user_id             INT PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    preferred_locale    VARCHAR(10)  NOT NULL DEFAULT 'fr',
    preferred_theme     VARCHAR(20)  NOT NULL DEFAULT 'system',
    created_at          TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE  auth.user_preferences IS 'Preferences UI par utilisateur (langue, theme, etc.)';
COMMENT ON COLUMN auth.user_preferences.preferred_locale IS 'Code langue (fr, en)';
COMMENT ON COLUMN auth.user_preferences.preferred_theme  IS 'Theme UI (light, dark, system)';
