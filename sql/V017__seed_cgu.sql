-- ============================================================================
-- V017__seed_cgu.sql
-- Kouroukan - Seed de la premiere version des CGU
-- ============================================================================

INSERT INTO auth.cgu_versions (version, contenu, date_publication, est_active) VALUES
('1.0', '# Conditions Generales d''Utilisation de Kouroukan

**Version 1.0 — Date d''effet : 1er septembre 2025**

## 1. Objet

Les presentes Conditions Generales d''Utilisation (ci-apres « CGU ») regissent l''acces et l''utilisation de la plateforme numerique **Kouroukan** (ci-apres « la Plateforme »), editee et exploitee par la societe Kouroukan SARL, immatriculee en Republique de Guinee.

La Plateforme est un service de gestion d''etablissement scolaire couvrant le pre-scolaire, le primaire, le secondaire et l''universitaire, accessible via les domaines www.kouroukan.gn et app.kouroukan.gn.

## 2. Acceptation des CGU

L''utilisation de la Plateforme est subordonnee a l''acceptation prealable et sans reserve des presentes CGU. En accedant a la Plateforme, l''Utilisateur reconnait avoir pris connaissance des presentes CGU et les accepter integralement.

L''acceptation des CGU est enregistree de maniere horodatee et constitue un engagement contractuel entre l''Utilisateur et Kouroukan SARL.

## 3. Definitions

- **Utilisateur** : toute personne physique disposant d''un compte sur la Plateforme (directeur, enseignant, parent, eleve, personnel administratif).
- **Etablissement** : toute institution scolaire ou universitaire utilisant la Plateforme.
- **Donnees personnelles** : toute information relative a une personne physique identifiee ou identifiable.
- **Contenu** : toute information, donnee, texte, image ou document transmis ou genere via la Plateforme.

## 4. Inscription et compte utilisateur

4.1. L''acces a la Plateforme necessite la creation d''un compte utilisateur par l''administrateur de l''etablissement ou par auto-inscription selon la configuration de l''etablissement.

4.2. L''Utilisateur s''engage a fournir des informations exactes et a les maintenir a jour.

4.3. L''Utilisateur est seul responsable de la confidentialite de ses identifiants de connexion. Toute utilisation de son compte est reputee faite par lui.

4.4. En cas de perte ou de compromission de ses identifiants, l''Utilisateur doit en informer immediatement l''administrateur de son etablissement.

## 5. Protection des donnees personnelles

5.1. Kouroukan SARL s''engage a proteger les donnees personnelles des Utilisateurs conformement a la legislation guineenne en vigueur en matiere de protection des donnees.

5.2. Les donnees collectees sont strictement necessaires au fonctionnement de la Plateforme et a la gestion scolaire.

5.3. Les donnees des eleves mineurs font l''objet d''une protection renforcee. Leur collecte est realisee avec le consentement du parent ou tuteur legal.

5.4. Les donnees personnelles ne sont jamais vendues a des tiers. Elles peuvent etre partagees avec les autorites educatives competentes (MENA, METFP-ET, MESRS) dans le cadre de leurs missions legales.

5.5. L''Utilisateur dispose d''un droit d''acces, de rectification et de suppression de ses donnees en contactant l''administrateur de son etablissement ou le support Kouroukan.

## 6. Utilisation de la Plateforme

6.1. L''Utilisateur s''engage a utiliser la Plateforme de maniere loyale, conformement a sa destination et aux presentes CGU.

6.2. Sont notamment interdits :
- L''utilisation de la Plateforme a des fins illegales ou frauduleuses
- La tentative d''acces non autorise aux donnees d''autres utilisateurs ou etablissements
- La diffusion de contenu diffamatoire, injurieux ou contraire a l''ordre public
- La modification, la decompilation ou le desassemblage de la Plateforme
- L''utilisation de robots, scripts ou tout moyen automatise d''acces

## 7. Propriete intellectuelle

7.1. La Plateforme, son architecture, son code source, ses interfaces et sa documentation sont la propriete exclusive de Kouroukan SARL.

7.2. Les contenus pedagogiques saisis par les enseignants restent la propriete de leurs auteurs respectifs.

7.3. Les donnees scolaires (notes, bulletins, absences) sont la propriete de l''etablissement concerne.

## 8. Disponibilite et maintenance

8.1. Kouroukan SARL s''efforce d''assurer la disponibilite de la Plateforme 24h/24, 7j/7, sous reserve des periodes de maintenance.

8.2. Kouroukan SARL ne saurait etre tenu responsable des interruptions liees a des problemes de connectivite internet, frequents en Guinee.

8.3. En cas d''indisponibilite prolongee, les Utilisateurs seront informes par les canaux de communication disponibles.

## 9. Responsabilite

9.1. Kouroukan SARL met en oeuvre tous les moyens raisonnables pour assurer la securite et la fiabilite de la Plateforme.

9.2. Kouroukan SARL ne saurait etre tenu responsable des dommages indirects resultant de l''utilisation de la Plateforme.

9.3. L''Etablissement reste seul responsable de l''exactitude des donnees scolaires saisies sur la Plateforme.

## 10. Tarification

10.1. L''offre STARTER est gratuite et permet la gestion de jusqu''a 100 eleves.

10.2. Les offres STANDARD et PREMIUM font l''objet d''une tarification sur devis, communiquee a l''etablissement avant souscription.

10.3. Les services premium pour parents (notifications SMS, alertes, rapports) sont factures selon la grille tarifaire en vigueur.

## 11. Modification des CGU

11.1. Kouroukan SARL se reserve le droit de modifier les presentes CGU a tout moment.

11.2. Les Utilisateurs seront informes de toute modification par notification dans la Plateforme.

11.3. La poursuite de l''utilisation de la Plateforme apres modification des CGU vaut acceptation des nouvelles conditions.

## 12. Resiliation

12.1. L''Utilisateur peut demander la cloture de son compte a tout moment aupres de l''administrateur de son etablissement.

12.2. Kouroukan SARL se reserve le droit de suspendre ou de cloturer un compte en cas de violation des presentes CGU.

## 13. Droit applicable et litiges

13.1. Les presentes CGU sont regies par le droit guineen.

13.2. En cas de litige, les parties s''efforceront de trouver une solution amiable. A defaut, les tribunaux de Conakry seront seuls competents.

## 14. Contact

Pour toute question relative aux presentes CGU :
- **Email** : contact@kouroukan.gn
- **Telephone** : +224 XX XX XX XX
- **Adresse** : Conakry, Republique de Guinee
', NOW(), TRUE)
ON CONFLICT DO NOTHING;
