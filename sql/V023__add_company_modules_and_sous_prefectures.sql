-- ============================================================================
-- V023__add_company_modules_and_sous_prefectures.sql
-- Kouroukan - Ajout des modules/school_name sur auth.companies
--             + Seed des sous-prefectures de Guinee
-- ============================================================================

-- ============================================================================
-- 1. ALTER auth.companies — ajout colonnes pour l'inscription en ligne
-- ============================================================================

ALTER TABLE auth.companies
    ADD COLUMN IF NOT EXISTS modules      TEXT[]          NOT NULL DEFAULT '{}',
    ADD COLUMN IF NOT EXISTS school_name  VARCHAR(300)    NULL;

-- ============================================================================
-- 2. SEED geo.sous_prefectures
--    Conakry (CKY) : zones speciales — les "prefectures" sont des communes,
--    il n'y a PAS de sous-prefectures pour Conakry.
-- ============================================================================

-- ── Region BOKE ──────────────────────────────────────────────────────────────

-- Prefecture Boke (BOK)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Boke Centre',   'BKC', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Binari',        'BNR', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Camayenne',     'CMY', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Kanfarande',    'KNF', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Kolaboui',      'KLB', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Mankountan',    'MNK', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Sangaredi',     'SGR', (SELECT id FROM geo.prefectures WHERE code = 'BOK')),
    ('Tanene',        'TNE', (SELECT id FROM geo.prefectures WHERE code = 'BOK'))
ON CONFLICT DO NOTHING;

-- Prefecture Boffa (BFA)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Boffa Centre',  'BFC', (SELECT id FROM geo.prefectures WHERE code = 'BFA')),
    ('Baguinet',      'BGN', (SELECT id FROM geo.prefectures WHERE code = 'BFA')),
    ('Douprou',       'DPR', (SELECT id FROM geo.prefectures WHERE code = 'BFA')),
    ('Koba',          'KBA', (SELECT id FROM geo.prefectures WHERE code = 'BFA')),
    ('Lisso',         'LSO', (SELECT id FROM geo.prefectures WHERE code = 'BFA'))
ON CONFLICT DO NOTHING;

-- Prefecture Fria (FRI)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Fria Centre',   'FRC', (SELECT id FROM geo.prefectures WHERE code = 'FRI')),
    ('Banguignyi',    'BGY', (SELECT id FROM geo.prefectures WHERE code = 'FRI')),
    ('Tormelin',      'TRM', (SELECT id FROM geo.prefectures WHERE code = 'FRI'))
ON CONFLICT DO NOTHING;

-- Prefecture Gaoual (GAO)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Gaoual Centre', 'GAC', (SELECT id FROM geo.prefectures WHERE code = 'GAO')),
    ('Foulamory',     'FLM', (SELECT id FROM geo.prefectures WHERE code = 'GAO')),
    ('Koumbia',       'KMB', (SELECT id FROM geo.prefectures WHERE code = 'GAO')),
    ('Touba',         'TBA', (SELECT id FROM geo.prefectures WHERE code = 'GAO')),
    ('Wendou-Mbour',  'WMB', (SELECT id FROM geo.prefectures WHERE code = 'GAO'))
ON CONFLICT DO NOTHING;

-- Prefecture Koundara (KND)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Koundara Centre', 'KDC', (SELECT id FROM geo.prefectures WHERE code = 'KND')),
    ('Beyla',           'BYK', (SELECT id FROM geo.prefectures WHERE code = 'KND')),
    ('Foulamori',       'FLK', (SELECT id FROM geo.prefectures WHERE code = 'KND')),
    ('Guingan',         'GNG', (SELECT id FROM geo.prefectures WHERE code = 'KND')),
    ('Manda',           'MDA', (SELECT id FROM geo.prefectures WHERE code = 'KND'))
ON CONFLICT DO NOTHING;

-- ── Region FARANAH ───────────────────────────────────────────────────────────

-- Prefecture Faranah (FAR)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Faranah Centre',  'FAC', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Banian',          'BNN', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Beindou',         'BND', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Gbangbadou',      'GBD', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Heremakono',      'HRM', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Kobikoro',        'KBK', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Sandenia',        'SDN', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Songoyah',        'SGY', (SELECT id FROM geo.prefectures WHERE code = 'FAR')),
    ('Tiro',            'TRO', (SELECT id FROM geo.prefectures WHERE code = 'FAR'))
ON CONFLICT DO NOTHING;

-- Prefecture Dabola (DAB)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Dabola Centre',   'DAC', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Arfamoussaya',    'ARM', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Banko',           'BNK', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Binia',           'BNA', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Bissikrima',      'BSK', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Dogomet',         'DGM', (SELECT id FROM geo.prefectures WHERE code = 'DAB')),
    ('Ndema',           'NDM', (SELECT id FROM geo.prefectures WHERE code = 'DAB'))
ON CONFLICT DO NOTHING;

-- Prefecture Dinguiraye (DIN)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Dinguiraye Centre', 'DNC', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Banora',            'DNB', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Dialakoro',         'DLK', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Diontou',           'DNT', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Gagnakali',         'GNK', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Kalinko',           'KLN', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Lansanaya',         'LNS', (SELECT id FROM geo.prefectures WHERE code = 'DIN')),
    ('Mory',              'MRY', (SELECT id FROM geo.prefectures WHERE code = 'DIN'))
ON CONFLICT DO NOTHING;

-- Prefecture Kissidougou (KIS)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Kissidougou Centre', 'KSC', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Albadaria',          'ALB', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Bardou',             'BDU', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Firawa',             'FRW', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Gbessoba',           'GBS', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Kondiadou',          'KDD', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Koundou',            'KDO', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Sangardo',           'SGD', (SELECT id FROM geo.prefectures WHERE code = 'KIS')),
    ('Yende-Millimou',     'YNM', (SELECT id FROM geo.prefectures WHERE code = 'KIS'))
ON CONFLICT DO NOTHING;

-- ── Region KANKAN ────────────────────────────────────────────────────────────

-- Prefecture Kankan (KAN)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Kankan Centre',     'KNC', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Baranama',          'BRN', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Cissela',           'CSL', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Djankana',          'DJN', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Fatako',            'FTK', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Koumana',           'KMN', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Missamana',         'MSM', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Morodou',           'MRD', (SELECT id FROM geo.prefectures WHERE code = 'KAN')),
    ('Tinti-Oulel',       'TNO', (SELECT id FROM geo.prefectures WHERE code = 'KAN'))
ON CONFLICT DO NOTHING;

-- Prefecture Kerouane (KER)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Kerouane Centre',   'KRC', (SELECT id FROM geo.prefectures WHERE code = 'KER')),
    ('Damaro',            'DMR', (SELECT id FROM geo.prefectures WHERE code = 'KER')),
    ('Banankoro',         'BNK', (SELECT id FROM geo.prefectures WHERE code = 'KER')),
    ('Komodou',           'KMD', (SELECT id FROM geo.prefectures WHERE code = 'KER')),
    ('Koumandou',         'KMU', (SELECT id FROM geo.prefectures WHERE code = 'KER')),
    ('Sengbedou',         'SGB', (SELECT id FROM geo.prefectures WHERE code = 'KER'))
ON CONFLICT DO NOTHING;

-- Prefecture Kouroussa (KOU)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Kouroussa Centre',  'KOC', (SELECT id FROM geo.prefectures WHERE code = 'KOU')),
    ('Babila',            'BBL', (SELECT id FROM geo.prefectures WHERE code = 'KOU')),
    ('Baro',              'BRO', (SELECT id FROM geo.prefectures WHERE code = 'KOU')),
    ('Doura',             'DRA', (SELECT id FROM geo.prefectures WHERE code = 'KOU')),
    ('Gberedou',          'GBR', (SELECT id FROM geo.prefectures WHERE code = 'KOU')),
    ('Sanguiana',         'SGA', (SELECT id FROM geo.prefectures WHERE code = 'KOU'))
ON CONFLICT DO NOTHING;

-- Prefecture Mandiana (MAN)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Mandiana Centre',   'MDC', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Bate-Nafadji',      'BTN', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Franwalia',         'FRN', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Karifamoriah',      'KRF', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Niantanina',        'NTN', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Saranko',           'SRK', (SELECT id FROM geo.prefectures WHERE code = 'MAN')),
    ('Yankasso',          'YNK', (SELECT id FROM geo.prefectures WHERE code = 'MAN'))
ON CONFLICT DO NOTHING;

-- Prefecture Siguiri (SIG)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Siguiri Centre',    'SGC', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Doko',              'DKO', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Kintinian',         'KNT', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Niagassola',        'NGS', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Norassoba',         'NRS', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Sanguiana',         'SGS', (SELECT id FROM geo.prefectures WHERE code = 'SIG')),
    ('Won',               'WON', (SELECT id FROM geo.prefectures WHERE code = 'SIG'))
ON CONFLICT DO NOTHING;

-- ── Region KINDIA ────────────────────────────────────────────────────────────

-- Prefecture Kindia (KIN)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Kindia Centre',     'KIC', (SELECT id FROM geo.prefectures WHERE code = 'KIN')),
    ('Bangouya',          'BGY', (SELECT id FROM geo.prefectures WHERE code = 'KIN')),
    ('Damakania',         'DMK', (SELECT id FROM geo.prefectures WHERE code = 'KIN')),
    ('Friguiagbe',        'FRG', (SELECT id FROM geo.prefectures WHERE code = 'KIN')),
    ('Kollet',            'KLL', (SELECT id FROM geo.prefectures WHERE code = 'KIN')),
    ('Mambia',            'MMB', (SELECT id FROM geo.prefectures WHERE code = 'KIN'))
ON CONFLICT DO NOTHING;

-- Prefecture Coyah (COY)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Coyah Centre',      'COC', (SELECT id FROM geo.prefectures WHERE code = 'COY')),
    ('Mafanco',           'MFC', (SELECT id FROM geo.prefectures WHERE code = 'COY')),
    ('Manéah',            'MNH', (SELECT id FROM geo.prefectures WHERE code = 'COY')),
    ('Wonkifong',         'WKF', (SELECT id FROM geo.prefectures WHERE code = 'COY'))
ON CONFLICT DO NOTHING;

-- Prefecture Dubreka (DUB)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Dubreka Centre',    'DBC', (SELECT id FROM geo.prefectures WHERE code = 'DUB')),
    ('Badi',              'BDI', (SELECT id FROM geo.prefectures WHERE code = 'DUB')),
    ('Khorira',           'KHR', (SELECT id FROM geo.prefectures WHERE code = 'DUB')),
    ('Tanene',            'TNN', (SELECT id FROM geo.prefectures WHERE code = 'DUB'))
ON CONFLICT DO NOTHING;

-- Prefecture Forecariah (FOR)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Forecariah Centre', 'FOC', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Benty',             'BTY', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Farmoriah',         'FRM', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Kakossa',           'KKS', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Kaback',            'KBK', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Maferenya',         'MFR', (SELECT id FROM geo.prefectures WHERE code = 'FOR')),
    ('Sansale',           'SNS', (SELECT id FROM geo.prefectures WHERE code = 'FOR'))
ON CONFLICT DO NOTHING;

-- Prefecture Telimele (TEL)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Telimele Centre',   'TLC', (SELECT id FROM geo.prefectures WHERE code = 'TEL')),
    ('Ditinn',            'DTN', (SELECT id FROM geo.prefectures WHERE code = 'TEL')),
    ('Gongoret',          'GGR', (SELECT id FROM geo.prefectures WHERE code = 'TEL')),
    ('Kollet',            'KLT', (SELECT id FROM geo.prefectures WHERE code = 'TEL')),
    ('Lansanaya',         'LNA', (SELECT id FROM geo.prefectures WHERE code = 'TEL')),
    ('Mombeyah',          'MMY', (SELECT id FROM geo.prefectures WHERE code = 'TEL'))
ON CONFLICT DO NOTHING;

-- ── Region LABE ──────────────────────────────────────────────────────────────

-- Prefecture Labe (LAB)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Labe Centre',       'LAC', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Alpha-Yaya',        'ALY', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Beindou',           'BDL', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Daara',             'DAR', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Dionfo',            'DNF', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Hafia',             'HAF', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Kouramangui',       'KRM', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Noussy',            'NSY', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Sannou',            'SNN', (SELECT id FROM geo.prefectures WHERE code = 'LAB')),
    ('Tountouroun',       'TNT', (SELECT id FROM geo.prefectures WHERE code = 'LAB'))
ON CONFLICT DO NOTHING;

-- Prefecture Koubia (KBA)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Koubia Centre',     'KBC', (SELECT id FROM geo.prefectures WHERE code = 'KBA')),
    ('Hidayatou',         'HDY', (SELECT id FROM geo.prefectures WHERE code = 'KBA')),
    ('Kindoye',           'KDY', (SELECT id FROM geo.prefectures WHERE code = 'KBA')),
    ('Missira',           'MSR', (SELECT id FROM geo.prefectures WHERE code = 'KBA')),
    ('Timbi-Madina',      'TBM', (SELECT id FROM geo.prefectures WHERE code = 'KBA'))
ON CONFLICT DO NOTHING;

-- Prefecture Lelouma (LEL)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Lelouma Centre',    'LLC', (SELECT id FROM geo.prefectures WHERE code = 'LEL')),
    ('Diari',             'DAI', (SELECT id FROM geo.prefectures WHERE code = 'LEL')),
    ('Gueme-Barambem',    'GMB', (SELECT id FROM geo.prefectures WHERE code = 'LEL')),
    ('Kebali',            'KBL', (SELECT id FROM geo.prefectures WHERE code = 'LEL')),
    ('Linsan-Koura',      'LNK', (SELECT id FROM geo.prefectures WHERE code = 'LEL')),
    ('Manda',             'MDL', (SELECT id FROM geo.prefectures WHERE code = 'LEL'))
ON CONFLICT DO NOTHING;

-- Prefecture Mali (MAL)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Mali Centre',       'MLC', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Donghol-Touma',     'DGT', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Fougou',            'FGO', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Gayah',             'GYH', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Kansangui',         'KSG', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Laboya',            'LBY', (SELECT id FROM geo.prefectures WHERE code = 'MAL')),
    ('Madina-Wora',       'MDW', (SELECT id FROM geo.prefectures WHERE code = 'MAL'))
ON CONFLICT DO NOTHING;

-- Prefecture Tougue (TOU)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Tougue Centre',     'TGC', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Bouliwel',          'BLW', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Dopili',            'DPL', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Fatako',            'FTO', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Koba-Tarikol',      'KBT', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Kolangui',          'KLG', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Linsan',            'LNT', (SELECT id FROM geo.prefectures WHERE code = 'TOU')),
    ('Loudou',            'LDD', (SELECT id FROM geo.prefectures WHERE code = 'TOU'))
ON CONFLICT DO NOTHING;

-- ── Region MAMOU ─────────────────────────────────────────────────────────────

-- Prefecture Mamou (MAM)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Mamou Centre',      'MMC', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Bouliwel',          'BWT', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Dinka',             'DNK', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Dounet',            'DNE', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Gongoret',          'GGM', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Pelel',             'PPL', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Porédaka',          'PRD', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Saramoussaya',      'SRM', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Soyah',             'SYH', (SELECT id FROM geo.prefectures WHERE code = 'MAM')),
    ('Timbi-Touni',       'TBT', (SELECT id FROM geo.prefectures WHERE code = 'MAM'))
ON CONFLICT DO NOTHING;

-- Prefecture Dalaba (DAL)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Dalaba Centre',     'DLC', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Diari',             'DLD', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Ditinn',            'DLT', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Kalinko',           'KLD', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Koba',              'KBD', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Maci',              'MCI', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Minthin',           'MTH', (SELECT id FROM geo.prefectures WHERE code = 'DAL')),
    ('Sannou',            'SNL', (SELECT id FROM geo.prefectures WHERE code = 'DAL'))
ON CONFLICT DO NOTHING;

-- Prefecture Pita (PIT)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Pita Centre',       'PTC', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Bantingnel',        'BNL', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Bourouwal-Tappe',   'BTP', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Donghol-Sigon',     'DGS', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Keneya',            'KNY', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Kollet',            'KLP', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Maci',              'MCP', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Pelel',             'PLP', (SELECT id FROM geo.prefectures WHERE code = 'PIT')),
    ('Sannou',            'SNP', (SELECT id FROM geo.prefectures WHERE code = 'PIT'))
ON CONFLICT DO NOTHING;

-- ── Region NZEREKORE ─────────────────────────────────────────────────────────

-- Prefecture Nzerekore (NZR)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Nzerekore Centre',  'NZC', (SELECT id FROM geo.prefectures WHERE code = 'NZR')),
    ('Bossou',            'BSS', (SELECT id FROM geo.prefectures WHERE code = 'NZR')),
    ('Bounouma',          'BNM', (SELECT id FROM geo.prefectures WHERE code = 'NZR')),
    ('Gouecke',           'GCK', (SELECT id FROM geo.prefectures WHERE code = 'NZR')),
    ('Laine',             'LNE', (SELECT id FROM geo.prefectures WHERE code = 'NZR')),
    ('Wome',              'WME', (SELECT id FROM geo.prefectures WHERE code = 'NZR'))
ON CONFLICT DO NOTHING;

-- Prefecture Beyla (BEY)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Beyla Centre',      'BYC', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Blama',             'BLM', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Daro',              'DRO', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Diassodou',         'DSS', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Fouala',            'FLA', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Koyama',            'KYM', (SELECT id FROM geo.prefectures WHERE code = 'BEY')),
    ('Sinko',             'SNK', (SELECT id FROM geo.prefectures WHERE code = 'BEY'))
ON CONFLICT DO NOTHING;

-- Prefecture Gueckedou (GUE)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Gueckedou Centre',  'GCC', (SELECT id FROM geo.prefectures WHERE code = 'GUE')),
    ('Fangamadou',        'FGM', (SELECT id FROM geo.prefectures WHERE code = 'GUE')),
    ('Kassadou',          'KSD', (SELECT id FROM geo.prefectures WHERE code = 'GUE')),
    ('Nongoa',            'NGA', (SELECT id FROM geo.prefectures WHERE code = 'GUE')),
    ('Termessadou',       'TSD', (SELECT id FROM geo.prefectures WHERE code = 'GUE')),
    ('Yende-Millimou',    'YML', (SELECT id FROM geo.prefectures WHERE code = 'GUE'))
ON CONFLICT DO NOTHING;

-- Prefecture Lola (LOL)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Lola Centre',       'LOC', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('Bossou',            'BSL', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('Foumbadou',         'FMB', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('Kouankan',          'KNK', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('Koyama',            'KYL', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('N''Zoo',            'NZO', (SELECT id FROM geo.prefectures WHERE code = 'LOL')),
    ('Tounkarata',        'TNK', (SELECT id FROM geo.prefectures WHERE code = 'LOL'))
ON CONFLICT DO NOTHING;

-- Prefecture Macenta (MAC)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Macenta Centre',    'MCC', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Banie',             'BNE', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Bofossou',          'BFS', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Gbakedou',          'GBK', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Kouankan',          'KNM', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Panziazou',         'PNZ', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Vasseredou',        'VSR', (SELECT id FROM geo.prefectures WHERE code = 'MAC')),
    ('Wonde',             'WND', (SELECT id FROM geo.prefectures WHERE code = 'MAC'))
ON CONFLICT DO NOTHING;

-- Prefecture Yomou (YOM)
INSERT INTO geo.sous_prefectures (name, code, prefecture_id) VALUES
    ('Yomou Centre',      'YMC', (SELECT id FROM geo.prefectures WHERE code = 'YOM')),
    ('Bolodou',           'BLD', (SELECT id FROM geo.prefectures WHERE code = 'YOM')),
    ('Daro',              'DRY', (SELECT id FROM geo.prefectures WHERE code = 'YOM')),
    ('Diecke',            'DKE', (SELECT id FROM geo.prefectures WHERE code = 'YOM')),
    ('Gama-Berema',       'GMR', (SELECT id FROM geo.prefectures WHERE code = 'YOM')),
    ('Koule',             'KLE', (SELECT id FROM geo.prefectures WHERE code = 'YOM'))
ON CONFLICT DO NOTHING;
