"""
Génération du Guide d'Utilisation — Kouroukan
"""
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import cm
from reportlab.lib.colors import HexColor, white, black
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    HRFlowable, PageBreak, KeepTogether
)
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_JUSTIFY
from reportlab.platypus import Flowable
import os

# ─── Couleurs ──────────────────────────────────────────────────────────────
GREEN_DARK   = HexColor("#15803d")   # green-700
GREEN_LIGHT  = HexColor("#dcfce7")   # green-100
GREEN_MED    = HexColor("#22c55e")   # green-500
GRAY_DARK    = HexColor("#1e293b")   # slate-900
GRAY_MED     = HexColor("#475569")   # slate-600
GRAY_LIGHT   = HexColor("#f1f5f9")   # slate-100
BLUE_LIGHT   = HexColor("#dbeafe")   # blue-100
BLUE_DARK    = HexColor("#1d4ed8")   # blue-700
INDIGO_LIGHT = HexColor("#e0e7ff")
INDIGO_DARK  = HexColor("#4338ca")
VIOLET_LIGHT = HexColor("#ede9fe")
VIOLET_DARK  = HexColor("#7c3aed")
ORANGE_LIGHT = HexColor("#ffedd5")
ORANGE_DARK  = HexColor("#c2410c")
SKY_LIGHT    = HexColor("#e0f2fe")
SKY_DARK     = HexColor("#0369a1")
AMBER_LIGHT  = HexColor("#fef3c7")
AMBER_DARK   = HexColor("#b45309")
PINK_LIGHT   = HexColor("#fce7f3")
PINK_DARK    = HexColor("#be185d")
TEAL_LIGHT   = HexColor("#ccfbf1")
TEAL_DARK    = HexColor("#0f766e")
PURPLE_LIGHT = HexColor("#f5f3ff")
PURPLE_DARK  = HexColor("#7e22ce")
YELLOW_LIGHT = HexColor("#fefce8")
YELLOW_DARK  = HexColor("#a16207")
BORDER_COLOR = HexColor("#e2e8f0")
WHITE        = white

# ─── Styles ─────────────────────────────────────────────────────────────────
def build_styles():
    base = getSampleStyleSheet()

    styles = {
        "cover_title": ParagraphStyle(
            "cover_title", fontName="Helvetica-Bold",
            fontSize=36, leading=44, textColor=WHITE, alignment=TA_CENTER, spaceAfter=6
        ),
        "cover_sub": ParagraphStyle(
            "cover_sub", fontName="Helvetica",
            fontSize=16, leading=22, textColor=WHITE, alignment=TA_CENTER, spaceAfter=4
        ),
        "cover_version": ParagraphStyle(
            "cover_version", fontName="Helvetica",
            fontSize=11, leading=14, textColor=HexColor("#bbf7d0"), alignment=TA_CENTER
        ),

        "h1": ParagraphStyle(
            "h1", fontName="Helvetica-Bold",
            fontSize=20, leading=26, textColor=GREEN_DARK,
            spaceBefore=18, spaceAfter=8
        ),
        "h2": ParagraphStyle(
            "h2", fontName="Helvetica-Bold",
            fontSize=14, leading=18, textColor=GRAY_DARK,
            spaceBefore=14, spaceAfter=6
        ),
        "h3": ParagraphStyle(
            "h3", fontName="Helvetica-Bold",
            fontSize=11, leading=15, textColor=GRAY_DARK,
            spaceBefore=10, spaceAfter=4
        ),
        "body": ParagraphStyle(
            "body", fontName="Helvetica",
            fontSize=10, leading=15, textColor=GRAY_MED,
            spaceBefore=2, spaceAfter=4, alignment=TA_JUSTIFY
        ),
        "body_bold": ParagraphStyle(
            "body_bold", fontName="Helvetica-Bold",
            fontSize=10, leading=15, textColor=GRAY_DARK,
            spaceBefore=2, spaceAfter=4
        ),
        "bullet": ParagraphStyle(
            "bullet", fontName="Helvetica",
            fontSize=10, leading=14, textColor=GRAY_MED,
            spaceBefore=1, spaceAfter=1,
            leftIndent=16, bulletIndent=4, alignment=TA_LEFT
        ),
        "caption": ParagraphStyle(
            "caption", fontName="Helvetica-Oblique",
            fontSize=9, leading=12, textColor=GRAY_MED, alignment=TA_CENTER
        ),
        "toc_title": ParagraphStyle(
            "toc_title", fontName="Helvetica-Bold",
            fontSize=18, leading=24, textColor=GRAY_DARK,
            spaceBefore=0, spaceAfter=12, alignment=TA_LEFT
        ),
        "toc_entry": ParagraphStyle(
            "toc_entry", fontName="Helvetica",
            fontSize=10, leading=18, textColor=GRAY_MED
        ),
        "toc_entry_main": ParagraphStyle(
            "toc_entry_main", fontName="Helvetica-Bold",
            fontSize=10, leading=18, textColor=GRAY_DARK
        ),
        "badge_text": ParagraphStyle(
            "badge_text", fontName="Helvetica-Bold",
            fontSize=8, leading=10, textColor=WHITE, alignment=TA_CENTER
        ),
        "section_num": ParagraphStyle(
            "section_num", fontName="Helvetica-Bold",
            fontSize=28, leading=32, textColor=GREEN_LIGHT, alignment=TA_LEFT
        ),
        "info_box": ParagraphStyle(
            "info_box", fontName="Helvetica",
            fontSize=9.5, leading=14, textColor=BLUE_DARK, alignment=TA_JUSTIFY
        ),
        "warning_box": ParagraphStyle(
            "warning_box", fontName="Helvetica",
            fontSize=9.5, leading=14, textColor=AMBER_DARK, alignment=TA_JUSTIFY
        ),
        "tip_box": ParagraphStyle(
            "tip_box", fontName="Helvetica",
            fontSize=9.5, leading=14, textColor=TEAL_DARK, alignment=TA_JUSTIFY
        ),
    }
    return styles

# ─── Helpers ────────────────────────────────────────────────────────────────
def colored_box(content_paragraphs, bg_color, border_color, padding=10):
    """Wrap paragraphs in a colored background table."""
    data = [[p] for p in content_paragraphs]
    t = Table([[Paragraph("", ParagraphStyle("x"))] + [p for p in content_paragraphs]],
              colWidths=[0])
    # Simpler: just a 1-column table
    inner = Table([[p] for p in content_paragraphs], colWidths=[15.5*cm])
    inner.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), bg_color),
        ("LEFTPADDING",  (0, 0), (-1, -1), padding),
        ("RIGHTPADDING", (0, 0), (-1, -1), padding),
        ("TOPPADDING",   (0, 0), (0, 0),  padding),
        ("BOTTOMPADDING",(0, -1), (-1, -1), padding),
        ("TOPPADDING",   (0, 1), (-1, -1), 2),
        ("BOTTOMPADDING",(0, 0), (-1, -2), 2),
        ("BOX", (0, 0), (-1, -1), 1, border_color),
        ("ROUNDEDCORNERS", [4]),
    ]))
    return inner

def info_box(text, styles, kind="info"):
    icons = {"info": "ℹ", "tip": "✓", "warning": "⚠"}
    colors = {
        "info":    (BLUE_LIGHT,   HexColor("#93c5fd"),  styles["info_box"]),
        "tip":     (TEAL_LIGHT,   HexColor("#5eead4"),  styles["tip_box"]),
        "warning": (AMBER_LIGHT,  HexColor("#fcd34d"),  styles["warning_box"]),
    }
    bg, border, style = colors[kind]
    icon = icons[kind]
    p = Paragraph(f"<b>{icon}</b>  {text}", style)
    return colored_box([p], bg, border)

def role_badge_table(roles_data, styles):
    """roles_data: list of (label, bg_color, text_color)"""
    cells = []
    for label, bg, tc in roles_data:
        p = ParagraphStyle("badge", fontName="Helvetica-Bold",
                           fontSize=8, leading=10, textColor=tc, alignment=TA_CENTER)
        cells.append(Paragraph(label, p))
    t = Table([cells], colWidths=[2.8*cm]*len(cells) if len(cells) <= 5 else [2.2*cm]*len(cells))
    style_cmds = [
        ("ALIGN", (0, 0), (-1, -1), "CENTER"),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
        ("LEFTPADDING", (0, 0), (-1, -1), 4),
        ("RIGHTPADDING", (0, 0), (-1, -1), 4),
        ("ROUNDEDCORNERS", [4]),
    ]
    for i, (_, bg, _) in enumerate(roles_data):
        style_cmds.append(("BACKGROUND", (i, 0), (i, 0), bg))
    t.setStyle(TableStyle(style_cmds))
    return t

def section_header(number, title, subtitle, styles):
    """Large decorated section header."""
    title_p = Paragraph(title, ParagraphStyle(
        "sh_title", fontName="Helvetica-Bold",
        fontSize=22, leading=28, textColor=WHITE
    ))
    if subtitle:
        sub_p = Paragraph(subtitle, ParagraphStyle(
            "sh_sub", fontName="Helvetica",
            fontSize=11, leading=15, textColor=HexColor("#bbf7d0")
        ))
        inner = Table([[title_p], [sub_p]], colWidths=[14*cm])
    else:
        inner = Table([[title_p]], colWidths=[14*cm])
    inner.setStyle(TableStyle([
        ("LEFTPADDING",  (0, 0), (-1, -1), 0),
        ("RIGHTPADDING", (0, 0), (-1, -1), 0),
        ("TOPPADDING",   (0, 0), (-1, -1), 0),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 4),
    ]))

    num_p = Paragraph(str(number), ParagraphStyle(
        "sh_num", fontName="Helvetica-Bold",
        fontSize=40, leading=44, textColor=HexColor("#166534"), alignment=TA_CENTER
    ))

    t = Table([[num_p, inner]], colWidths=[2*cm, 14*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), GREEN_DARK),
        ("LEFTPADDING",  (0, 0), (0, 0), 20),
        ("LEFTPADDING",  (1, 0), (1, 0), 10),
        ("RIGHTPADDING", (0, 0), (-1, -1), 20),
        ("TOPPADDING",   (0, 0), (-1, -1), 16),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 16),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("ROUNDEDCORNERS", [6]),
    ]))
    return t

def module_card(icon, title, description, sub_modules, bg, border, title_color, styles):
    """Card for a module."""
    header_p = Paragraph(f"{icon}  <b>{title}</b>", ParagraphStyle(
        "mc_h", fontName="Helvetica-Bold",
        fontSize=11, leading=14, textColor=title_color
    ))
    desc_p = Paragraph(description, ParagraphStyle(
        "mc_d", fontName="Helvetica",
        fontSize=9, leading=13, textColor=GRAY_MED
    ))
    # Sub-modules as bullet list
    sub_items = []
    for sm in sub_modules:
        sub_items.append(Paragraph(f"• {sm}", ParagraphStyle(
            "mc_sm", fontName="Helvetica",
            fontSize=9, leading=12, textColor=GRAY_MED, leftIndent=8
        )))

    all_content = [header_p, Spacer(1, 4), desc_p]
    if sub_items:
        all_content.append(Spacer(1, 4))
        all_content += sub_items

    t = Table([[p] for p in all_content], colWidths=[8.2*cm])
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), bg),
        ("BOX", (0, 0), (-1, -1), 1.5, border),
        ("LEFTPADDING",  (0, 0), (-1, -1), 12),
        ("RIGHTPADDING", (0, 0), (-1, -1), 12),
        ("TOPPADDING",   (0, 0), (0, 0), 12),
        ("BOTTOMPADDING",(0, -1), (-1, -1), 12),
        ("TOPPADDING",   (0, 1), (-1, -1), 3),
        ("BOTTOMPADDING",(0, 0), (-1, -2), 2),
        ("ROUNDEDCORNERS", [5]),
    ]))
    return t

def two_col_cards(card1, card2):
    spacer = Spacer(0.5*cm, 0.1)
    t = Table([[card1, spacer, card2]], colWidths=[8.2*cm, 0.5*cm, 8.2*cm])
    t.setStyle(TableStyle([
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING",  (0, 0), (-1, -1), 0),
        ("RIGHTPADDING", (0, 0), (-1, -1), 0),
        ("TOPPADDING",   (0, 0), (-1, -1), 0),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 0),
    ]))
    return t

def permission_table(data, col_widths, styles):
    """Generic styled table."""
    t = Table(data, colWidths=col_widths)
    cmds = [
        ("BACKGROUND", (0, 0), (-1, 0), GREEN_DARK),
        ("TEXTCOLOR",  (0, 0), (-1, 0), WHITE),
        ("FONTNAME",   (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE",   (0, 0), (-1, 0), 9),
        ("ALIGN",      (0, 0), (-1, -1), "LEFT"),
        ("VALIGN",     (0, 0), (-1, -1), "MIDDLE"),
        ("TOPPADDING", (0, 0), (-1, -1), 7),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 7),
        ("LEFTPADDING", (0, 0), (-1, -1), 10),
        ("RIGHTPADDING",(0, 0), (-1, -1), 10),
        ("FONTNAME",   (0, 1), (-1, -1), "Helvetica"),
        ("FONTSIZE",   (0, 1), (-1, -1), 8.5),
        ("TEXTCOLOR",  (0, 1), (-1, -1), GRAY_MED),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [WHITE, GRAY_LIGHT]),
        ("LINEBELOW",  (0, 0), (-1, -1), 0.5, BORDER_COLOR),
        ("BOX",        (0, 0), (-1, -1), 1, BORDER_COLOR),
        ("ROUNDEDCORNERS", [4]),
    ]
    t.setStyle(TableStyle(cmds))
    return t

# ─── Document ────────────────────────────────────────────────────────────────
def build_document():
    output_path = os.path.join(
        os.path.dirname(os.path.abspath(__file__)),
        "guide_utilisateur_kouroukan.pdf"
    )
    doc = SimpleDocTemplate(
        output_path,
        pagesize=A4,
        leftMargin=2*cm,
        rightMargin=2*cm,
        topMargin=2*cm,
        bottomMargin=2*cm,
        title="Guide d'Utilisation — Kouroukan",
        author="Kouroukan",
    )

    S = build_styles()
    W = 17*cm  # usable width
    story = []

    # ═══════════════════════════════════════════════════════════════════
    # PAGE DE COUVERTURE
    # ═══════════════════════════════════════════════════════════════════
    cover_logo = Paragraph("🎓", ParagraphStyle(
        "cl", fontName="Helvetica-Bold", fontSize=60,
        leading=70, textColor=WHITE, alignment=TA_CENTER
    ))
    cover_title = Paragraph("Kouroukan", S["cover_title"])
    cover_subtitle = Paragraph("Plateforme de Gestion d'Établissement Scolaire", S["cover_sub"])
    cover_sep = HRFlowable(width="60%", thickness=1, color=HexColor("#86efac"), spaceAfter=12)
    cover_guide = Paragraph("Guide d'Utilisation", ParagraphStyle(
        "cg", fontName="Helvetica-Bold",
        fontSize=20, leading=26, textColor=WHITE, alignment=TA_CENTER, spaceAfter=6
    ))
    cover_version = Paragraph("Version 1.0  •  Mars 2026", S["cover_version"])

    cover_inner = Table(
        [[cover_logo],
         [Spacer(1, 10)],
         [cover_title],
         [cover_subtitle],
         [Spacer(1, 6)],
         [cover_sep],
         [cover_guide],
         [cover_version],
         [Spacer(1, 40)]],
        colWidths=[W]
    )
    cover_inner.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), GREEN_DARK),
        ("TOPPADDING",   (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 4),
        ("LEFTPADDING",  (0, 0), (-1, -1), 30),
        ("RIGHTPADDING", (0, 0), (-1, -1), 30),
        ("TOPPADDING",   (0, 0), (0, 0), 50),
        ("ROUNDEDCORNERS", [10]),
    ]))
    story.append(cover_inner)
    story.append(Spacer(1, 20))

    # Cover info box
    cover_info_data = [
        [Paragraph("<b>À propos de ce guide</b>", S["body_bold"])],
        [Paragraph(
            "Ce guide d'utilisation est destiné à tous les utilisateurs de la plateforme Kouroukan. "
            "Il couvre l'ensemble des fonctionnalités disponibles selon votre rôle, ainsi que les procédures "
            "de connexion, de navigation et d'utilisation des différents modules.",
            S["body"]
        )],
    ]
    cover_info = Table(cover_info_data, colWidths=[W])
    cover_info.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), GREEN_LIGHT),
        ("BOX",  (0, 0), (-1, -1), 1, HexColor("#86efac")),
        ("LEFTPADDING",  (0, 0), (-1, -1), 16),
        ("RIGHTPADDING", (0, 0), (-1, -1), 16),
        ("TOPPADDING",   (0, 0), (-1, -1), 10),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 10),
        ("ROUNDEDCORNERS", [5]),
    ]))
    story.append(cover_info)
    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # TABLE DES MATIÈRES
    # ═══════════════════════════════════════════════════════════════════
    story.append(Paragraph("Table des Matières", S["toc_title"]))
    story.append(HRFlowable(width=W, thickness=2, color=GREEN_DARK, spaceAfter=10))

    toc_items = [
        ("1.", "Présentation de l'Application",          ["1.1 Vue d'ensemble", "1.2 Architecture fonctionnelle"]),
        ("2.", "Connexion et Authentification",           ["2.1 Se connecter", "2.2 Récupération de mot de passe", "2.3 CGU — Conditions Générales d'Utilisation"]),
        ("3.", "Profils Utilisateurs et Rôles",           ["3.1 Tableau des rôles", "3.2 Description de chaque profil"]),
        ("4.", "Navigation et Interface",                 ["4.1 Tableau de bord", "4.2 Menu de navigation latéral", "4.3 En-tête et menu utilisateur"]),
        ("5.", "Modules Fonctionnels",                    ["5.1 Inscriptions", "5.2 Pédagogie", "5.3 Évaluations", "5.4 Présences", "5.5 Finances", "5.6 Personnel", "5.7 Communication", "5.8 BDE", "5.9 Documents", "5.10 Services Premium"]),
        ("6.", "Support et Aide",                         ["6.1 Tickets de support", "6.2 Suggestions", "6.3 Documentation", "6.4 Assistant IA"]),
        ("7.", "Mon Profil et Paramètres",                ["7.1 Modifier mon profil", "7.2 Changer de mot de passe", "7.3 Préférences d'affichage"]),
        ("8.", "Référence Rapide par Type d'Utilisateur", ["Administrateurs", "Direction", "Enseignants", "Personnel administratif", "Parents & Élèves"]),
    ]

    for num, title, subs in toc_items:
        row_data = [[
            Paragraph(num, ParagraphStyle("tn", fontName="Helvetica-Bold", fontSize=10,
                                          textColor=GREEN_DARK, leading=14)),
            Paragraph(title, S["toc_entry_main"]),
        ]]
        t = Table(row_data, colWidths=[0.7*cm, W-0.7*cm])
        t.setStyle(TableStyle([
            ("TOPPADDING",   (0, 0), (-1, -1), 4),
            ("BOTTOMPADDING",(0, 0), (-1, -1), 0),
            ("LEFTPADDING",  (0, 0), (-1, -1), 0),
            ("RIGHTPADDING", (0, 0), (-1, -1), 0),
        ]))
        story.append(t)
        for sub in subs:
            story.append(Paragraph(f"    — {sub}", S["toc_entry"]))
        story.append(Spacer(1, 3))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 1 — PRÉSENTATION
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("1", "Présentation de l'Application",
                                "Vue d'ensemble de Kouroukan", S))
    story.append(Spacer(1, 16))

    story.append(Paragraph("1.1 Vue d'ensemble", S["h2"]))
    story.append(Paragraph(
        "Kouroukan est une plateforme numérique intégrée de gestion d'établissement scolaire. "
        "Elle centralise l'ensemble des processus administratifs, pédagogiques et financiers "
        "d'une école en un seul outil accessible depuis n'importe quel navigateur web.",
        S["body"]
    ))
    story.append(Spacer(1, 6))

    # Key features grid
    features = [
        ("📋", "Gestion des Inscriptions", "Suivi complet des dossiers d'élèves, admissions et années scolaires"),
        ("📚", "Pédagogie",               "Organisation des classes, matières, séances et cahiers de textes"),
        ("📊", "Évaluations",             "Saisie des notes, gestion des évaluations et édition des bulletins"),
        ("🕐", "Présences",               "Appels, suivi des absences et pointages électroniques"),
        ("💰", "Finances",                "Facturation, paiements, dépenses et rémunérations"),
        ("👥", "Personnel",               "Gestion des enseignants et demandes de congés"),
    ]
    feat_rows = []
    for i in range(0, len(features), 2):
        row = []
        for j in range(2):
            if i+j < len(features):
                icon, title_f, desc_f = features[i+j]
                cell = Table(
                    [[Paragraph(f"{icon}  <b>{title_f}</b>", ParagraphStyle(
                        "ft", fontName="Helvetica-Bold", fontSize=10, textColor=GRAY_DARK, leading=14
                      ))],
                     [Paragraph(desc_f, ParagraphStyle(
                        "fd", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=12
                      ))]],
                    colWidths=[8.2*cm]
                )
                cell.setStyle(TableStyle([
                    ("BACKGROUND", (0, 0), (-1, -1), GRAY_LIGHT),
                    ("BOX", (0, 0), (-1, -1), 1, BORDER_COLOR),
                    ("LEFTPADDING", (0, 0), (-1, -1), 12),
                    ("RIGHTPADDING", (0, 0), (-1, -1), 12),
                    ("TOPPADDING", (0, 0), (-1, -1), 8),
                    ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
                    ("TOPPADDING", (0, 1), (-1, -1), 3),
                ]))
                row.append(cell)
            else:
                row.append(Paragraph("", S["body"]))
        feat_rows.append(row)

    for feat_row in feat_rows:
        t = Table([feat_row], colWidths=[8.2*cm, 8.2*cm + 0.5*cm])
        t.setStyle(TableStyle([
            ("LEFTPADDING", (0, 0), (-1, -1), 0),
            ("RIGHTPADDING", (0, 0), (-1, -1), 0),
            ("TOPPADDING", (0, 0), (-1, -1), 0),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ]))
        story.append(t)

    story.append(Spacer(1, 10))
    story.append(Paragraph("1.2 Architecture fonctionnelle", S["h2"]))
    story.append(Paragraph(
        "La plateforme est organisée autour d'un système de <b>contrôle d'accès basé sur les rôles</b> (RBAC). "
        "Chaque utilisateur se voit attribuer un ou plusieurs rôles qui déterminent les modules visibles "
        "et les actions autorisées. Cette architecture garantit que chaque utilisateur n'accède qu'aux "
        "fonctionnalités pertinentes pour sa fonction.",
        S["body"]
    ))
    story.append(Spacer(1, 6))
    story.append(info_box(
        "La plateforme prend en charge <b>13 profils utilisateurs distincts</b> répartis en cinq grandes "
        "catégories : Administration, Direction, Corps enseignant, Personnel administratif, et Communauté scolaire (parents/élèves).",
        S, "info"
    ))
    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 2 — CONNEXION
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("2", "Connexion et Authentification",
                                "Accès sécurisé à la plateforme", S))
    story.append(Spacer(1, 16))

    story.append(Paragraph("2.1 Se connecter à la plateforme", S["h2"]))
    story.append(Paragraph(
        "Pour accéder à Kouroukan, ouvrez votre navigateur et saisissez l'adresse URL de votre "
        "établissement. Vous serez dirigé vers la page de connexion.",
        S["body"]
    ))
    story.append(Spacer(1, 8))

    # Steps numbered
    steps = [
        ("1", "Saisir votre adresse e-mail",
         "Entrez l'adresse e-mail qui vous a été communiquée par votre administrateur lors de la création de votre compte."),
        ("2", "Saisir votre mot de passe",
         "Entrez votre mot de passe. Celui-ci est sensible à la casse (majuscules/minuscules)."),
        ("3", "Cliquer sur « Se connecter »",
         "Validez vos informations en cliquant sur le bouton vert."),
        ("4", "Accepter les CGU (première connexion)",
         "Lors de votre première connexion, vous devrez lire et accepter les Conditions Générales d'Utilisation avant d'accéder à l'application."),
        ("5", "Accès au tableau de bord",
         "Après authentification, vous êtes redirigé vers le tableau de bord adapté à votre profil."),
    ]
    for num_s, title_s, desc_s in steps:
        step_num = Paragraph(num_s, ParagraphStyle(
            "sn", fontName="Helvetica-Bold", fontSize=14, textColor=WHITE,
            leading=18, alignment=TA_CENTER
        ))
        step_circle = Table([[step_num]], colWidths=[0.7*cm])
        step_circle.setStyle(TableStyle([
            ("BACKGROUND", (0, 0), (-1, -1), GREEN_DARK),
            ("TOPPADDING",   (0, 0), (-1, -1), 4),
            ("BOTTOMPADDING",(0, 0), (-1, -1), 4),
            ("LEFTPADDING",  (0, 0), (-1, -1), 6),
            ("RIGHTPADDING", (0, 0), (-1, -1), 6),
            ("ROUNDEDCORNERS", [14]),
        ]))
        step_content = Table(
            [[Paragraph(f"<b>{title_s}</b>", ParagraphStyle("st", fontName="Helvetica-Bold",
                         fontSize=10, textColor=GRAY_DARK, leading=14))],
             [Paragraph(desc_s, ParagraphStyle("sd", fontName="Helvetica",
                         fontSize=9.5, textColor=GRAY_MED, leading=13))]],
            colWidths=[15*cm]
        )
        step_content.setStyle(TableStyle([
            ("TOPPADDING",   (0, 0), (-1, -1), 2),
            ("BOTTOMPADDING",(0, 0), (-1, -1), 2),
            ("LEFTPADDING",  (0, 0), (-1, -1), 0),
            ("RIGHTPADDING", (0, 0), (-1, -1), 0),
        ]))
        row_t = Table([[step_circle, step_content]], colWidths=[1.2*cm, 15*cm])
        row_t.setStyle(TableStyle([
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING",  (0, 0), (-1, -1), 0),
            ("RIGHTPADDING", (0, 0), (-1, -1), 0),
            ("TOPPADDING",   (0, 0), (-1, -1), 4),
            ("BOTTOMPADDING",(0, 0), (-1, -1), 8),
        ]))
        story.append(row_t)

    story.append(Spacer(1, 6))
    story.append(info_box(
        "<b>Identifiants par défaut (admin système)</b> : email <b>admin@kouroukan.gn</b>, "
        "mot de passe <b>Admin@2024</b>. Changez ce mot de passe dès la première connexion.",
        S, "warning"
    ))

    story.append(Spacer(1, 12))
    story.append(Paragraph("2.2 Récupération de mot de passe", S["h2"]))
    story.append(Paragraph(
        "En cas d'oubli de mot de passe, contactez votre administrateur système ou le responsable "
        "informatique de votre établissement. Une procédure de réinitialisation vous sera communiquée "
        "par e-mail.",
        S["body"]
    ))

    story.append(Spacer(1, 12))
    story.append(Paragraph("2.3 Conditions Générales d'Utilisation (CGU)", S["h2"]))
    story.append(Paragraph(
        "À chaque mise à jour des CGU, tous les utilisateurs doivent les accepter avant de continuer "
        "à utiliser la plateforme. Cette acceptation est <b>obligatoire</b> et enregistrée dans le système "
        "avec la date et l'heure.",
        S["body"]
    ))
    story.append(Spacer(1, 6))
    story.append(info_box(
        "Les CGU sont accessibles en permanence via le menu : <b>Menu latéral → Support → CGU</b> "
        "ou directement à l'adresse <b>/legal/cgu</b>.",
        S, "tip"
    ))
    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 3 — PROFILS UTILISATEURS
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("3", "Profils Utilisateurs et Rôles",
                                "13 profils distincts avec des accès personnalisés", S))
    story.append(Spacer(1, 16))

    story.append(Paragraph("3.1 Tableau des rôles", S["h2"]))
    story.append(Paragraph(
        "Le tableau ci-dessous résume les 13 rôles disponibles, classés par niveau d'accès :",
        S["body"]
    ))
    story.append(Spacer(1, 8))

    roles_header = [
        Paragraph("Rôle", ParagraphStyle("rh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Libellé", ParagraphStyle("rh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Niveau d'accès", ParagraphStyle("rh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Description courte", ParagraphStyle("rh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
    ]
    roles_rows = [
        ("super_admin",           "Super Administrateur", "★★★★★", "Accès total à tous les modules et paramètres"),
        ("fondateur",             "Fondateur",            "★★★★☆", "Lecture complète, supervision générale"),
        ("admin_it",              "Administrateur IT",    "★★★★☆", "Gestion technique, utilisateurs, paramètres"),
        ("directeur",             "Directeur",            "★★★★☆", "Pilotage pédagogique et administratif complet"),
        ("censeur",               "Censeur",              "★★★☆☆", "Suivi pédagogique, évaluations, présences"),
        ("intendant",             "Intendant",            "★★★☆☆", "Gestion financière, factures, paiements"),
        ("responsable_admissions","Resp. Admissions",     "★★★☆☆", "Inscriptions, dossiers élèves"),
        ("chef_departement",      "Chef de Département",  "★★★☆☆", "Pédagogie et personnel de son département"),
        ("enseignant",            "Enseignant",           "★★☆☆☆", "Séances, notes, présences de ses classes"),
        ("personnel_admin",       "Personnel Admin.",     "★★☆☆☆", "Documents, communication, inscriptions"),
        ("responsable_bde",       "Resp. BDE",            "★★☆☆☆", "Gestion du bureau des élèves"),
        ("parent",                "Parent / Tuteur",      "★☆☆☆☆", "Consultation notes, absences, finances"),
        ("eleve",                 "Élève",                "★☆☆☆☆", "Consultation notes, emploi du temps, BDE"),
    ]

    mk = lambda txt, bold=False, color=GRAY_MED: Paragraph(
        txt,
        ParagraphStyle("c", fontName="Helvetica-Bold" if bold else "Helvetica",
                       fontSize=8.5, textColor=color, leading=12)
    )

    table_data = [roles_header]
    for role_id, label, level, desc in roles_rows:
        table_data.append([mk(role_id, bold=True, color=GREEN_DARK), mk(label, bold=True), mk(level), mk(desc)])

    roles_t = Table(table_data, colWidths=[3.5*cm, 3.5*cm, 3*cm, 7*cm])
    roles_t.setStyle(TableStyle([
        ("BACKGROUND",   (0, 0), (-1, 0),  GREEN_DARK),
        ("TEXTCOLOR",    (0, 0), (-1, 0),  WHITE),
        ("FONTNAME",     (0, 0), (-1, 0),  "Helvetica-Bold"),
        ("FONTSIZE",     (0, 0), (-1, 0),  9),
        ("ALIGN",        (0, 0), (-1, -1), "LEFT"),
        ("VALIGN",       (0, 0), (-1, -1), "MIDDLE"),
        ("TOPPADDING",   (0, 0), (-1, -1), 7),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 7),
        ("LEFTPADDING",  (0, 0), (-1, -1), 10),
        ("RIGHTPADDING", (0, 0), (-1, -1), 8),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [WHITE, GRAY_LIGHT]),
        ("LINEBELOW",    (0, 0), (-1, -1), 0.5, BORDER_COLOR),
        ("BOX",          (0, 0), (-1, -1), 1,   BORDER_COLOR),
        ("ROUNDEDCORNERS", [4]),
    ]))
    story.append(roles_t)
    story.append(PageBreak())

    # 3.2 Description des profils
    story.append(Paragraph("3.2 Description détaillée de chaque profil", S["h2"]))
    story.append(Spacer(1, 8))

    profiles = [
        {
            "icon": "👑", "title": "Super Administrateur (super_admin)",
            "color": GREEN_DARK, "bg": GREEN_LIGHT, "border": HexColor("#86efac"),
            "desc": "Le super administrateur dispose d'un accès illimité à l'ensemble de la plateforme. "
                    "Il peut créer, modifier et supprimer tout contenu, gérer les utilisateurs, "
                    "configurer les paramètres système et consulter les journaux d'audit.",
            "acces": [
                "Tous les modules sans exception",
                "Gestion des comptes utilisateurs",
                "Configuration système et paramètres",
                "Journaux d'audit et supervision",
                "Rechargement du cache système",
            ]
        },
        {
            "icon": "🏛️", "title": "Fondateur (fondateur)",
            "color": INDIGO_DARK, "bg": INDIGO_LIGHT, "border": HexColor("#a5b4fc"),
            "desc": "Le fondateur bénéficie d'un accès en lecture sur l'ensemble de la plateforme. "
                    "Il peut consulter tous les rapports, statistiques et données sans pouvoir "
                    "les modifier. Idéal pour un suivi stratégique de l'établissement.",
            "acces": [
                "Lecture de tous les modules",
                "Tableaux de bord et statistiques",
                "Journaux d'audit (lecture seule)",
                "Aucune modification possible",
            ]
        },
        {
            "icon": "💻", "title": "Administrateur IT (admin_it)",
            "color": BLUE_DARK, "bg": BLUE_LIGHT, "border": HexColor("#93c5fd"),
            "desc": "L'administrateur IT gère les aspects techniques de la plateforme : comptes "
                    "utilisateurs, droits d'accès, paramètres de configuration et maintenance du système. "
                    "Il assure le bon fonctionnement technique de la plateforme.",
            "acces": [
                "Gestion des comptes utilisateurs",
                "Configuration des paramètres système",
                "Modules : Inscriptions, Personnel, Documents",
                "Rechargement du cache Redis",
                "Journaux d'audit",
            ]
        },
        {
            "icon": "🎓", "title": "Directeur (directeur)",
            "color": GREEN_DARK, "bg": GREEN_LIGHT, "border": HexColor("#86efac"),
            "desc": "Le directeur est le principal gestionnaire de l'établissement sur la plateforme. "
                    "Il a accès à tous les modules opérationnels avec des droits de création, "
                    "modification et suppression sur l'ensemble des données scolaires.",
            "acces": [
                "Tous les modules opérationnels",
                "Gestion complète : inscriptions, pédagogie, évaluations",
                "Finances, personnel, documents",
                "Paramètres de l'établissement",
            ]
        },
        {
            "icon": "📋", "title": "Censeur (censeur)",
            "color": VIOLET_DARK, "bg": VIOLET_LIGHT, "border": HexColor("#c4b5fd"),
            "desc": "Le censeur est responsable du suivi pédagogique quotidien. Il supervise les "
                    "absences, les évaluations et la pédagogie. Il peut saisir des données mais "
                    "n'a pas accès aux fonctions financières.",
            "acces": [
                "Pédagogie : lecture et modification",
                "Évaluations : saisie et consultation",
                "Présences : gestion des appels et absences",
                "Communication : annonces et messages",
            ]
        },
        {
            "icon": "💼", "title": "Intendant (intendant)",
            "color": TEAL_DARK, "bg": TEAL_LIGHT, "border": HexColor("#5eead4"),
            "desc": "L'intendant gère la comptabilité et les finances de l'établissement. Il a accès "
                    "exclusif aux modules financiers et peut traiter les factures, paiements, dépenses "
                    "et rémunérations des enseignants.",
            "acces": [
                "Module Finances : accès complet",
                "Factures, paiements, dépenses",
                "Rémunérations des enseignants",
                "Services premium et souscriptions",
                "Documents financiers",
            ]
        },
        {
            "icon": "📝", "title": "Responsable Admissions (responsable_admissions)",
            "color": ORANGE_DARK, "bg": ORANGE_LIGHT, "border": HexColor("#fdba74"),
            "desc": "Le responsable des admissions gère l'ensemble du processus d'inscription des "
                    "nouveaux élèves, depuis la réception des dossiers jusqu'à la validation de "
                    "l'inscription définitive.",
            "acces": [
                "Module Inscriptions : accès complet",
                "Dossiers d'admission",
                "Registre des élèves",
                "Années scolaires (lecture)",
            ]
        },
        {
            "icon": "🏫", "title": "Chef de Département (chef_departement)",
            "color": PINK_DARK, "bg": PINK_LIGHT, "border": HexColor("#f9a8d4"),
            "desc": "Le chef de département coordonne son équipe pédagogique. Il accède aux "
                    "informations pédagogiques de son département et peut consulter le profil "
                    "de ses enseignants.",
            "acces": [
                "Pédagogie : lecture et modifications de département",
                "Personnel : consultation des enseignants",
                "Évaluations : consultation",
            ]
        },
        {
            "icon": "👨‍🏫", "title": "Enseignant (enseignant)",
            "color": SKY_DARK, "bg": SKY_LIGHT, "border": HexColor("#7dd3fc"),
            "desc": "L'enseignant accède aux outils nécessaires à sa pratique pédagogique quotidienne : "
                    "saisie des notes, gestion des présences dans ses classes, et accès au cahier "
                    "de textes.",
            "acces": [
                "Pédagogie : ses séances et classes",
                "Évaluations : saisie des notes de ses élèves",
                "Présences : appels de ses cours",
                "Communication : messages et annonces",
                "Demandes de congés",
            ]
        },
        {
            "icon": "🗂️", "title": "Personnel Administratif (personnel_admin)",
            "color": PURPLE_DARK, "bg": PURPLE_LIGHT, "border": HexColor("#c4b5fd"),
            "desc": "Le personnel administratif assiste la direction dans les tâches quotidiennes. "
                    "Il peut gérer les documents, les inscriptions et la communication interne.",
            "acces": [
                "Inscriptions : consultation et saisie",
                "Documents : génération et gestion",
                "Communication : messages",
                "Support : tickets et suggestions",
            ]
        },
        {
            "icon": "🎉", "title": "Responsable BDE (responsable_bde)",
            "color": AMBER_DARK, "bg": AMBER_LIGHT, "border": HexColor("#fcd34d"),
            "desc": "Le responsable du Bureau Des Élèves organise et gère les activités parascolaires "
                    "de l'établissement. Il peut gérer les associations, événements et le budget du BDE.",
            "acces": [
                "BDE : gestion complète",
                "Associations et événements",
                "Membres BDE et dépenses",
                "Support : tickets",
            ]
        },
        {
            "icon": "👨‍👩‍👧", "title": "Parent / Tuteur (parent)",
            "color": GREEN_DARK, "bg": GREEN_LIGHT, "border": HexColor("#86efac"),
            "desc": "Le parent ou tuteur légal suit la scolarité de son enfant via la plateforme. "
                    "Il a un accès en lecture aux informations pertinentes et peut consulter les "
                    "documents et suivre les finances.",
            "acces": [
                "Notes et bulletins de son enfant (lecture)",
                "Absences et présences (lecture)",
                "Factures et paiements de son enfant",
                "Services premium disponibles",
                "Communication : messagerie",
            ]
        },
        {
            "icon": "🎒", "title": "Élève (eleve)",
            "color": SKY_DARK, "bg": SKY_LIGHT, "border": HexColor("#7dd3fc"),
            "desc": "L'élève consulte ses propres résultats scolaires, son emploi du temps et "
                    "participe aux activités du BDE. Son accès est limité à ses données personnelles.",
            "acces": [
                "Ses notes et bulletins (lecture)",
                "Son emploi du temps",
                "BDE : participation aux activités",
                "Communication : messagerie",
            ]
        },
    ]

    for i in range(0, len(profiles), 2):
        cards_row = []
        for j in range(2):
            if i+j < len(profiles):
                p = profiles[i+j]
                acces_items = [Paragraph(f"• {a}", ParagraphStyle(
                    "pa", fontName="Helvetica", fontSize=9, textColor=p["color"],
                    leading=12, leftIndent=8
                )) for a in p["acces"]]
                header_p = Paragraph(f"{p['icon']}  <b>{p['title']}</b>", ParagraphStyle(
                    "ph", fontName="Helvetica-Bold", fontSize=10, textColor=p["color"], leading=14
                ))
                desc_p = Paragraph(p["desc"], ParagraphStyle(
                    "pd", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=13
                ))
                access_title = Paragraph("<b>Accès autorisés :</b>", ParagraphStyle(
                    "pat", fontName="Helvetica-Bold", fontSize=9, textColor=GRAY_DARK, leading=12
                ))

                rows = [[header_p], [Spacer(1, 4)], [desc_p], [Spacer(1, 6)], [access_title]]
                rows += [[item] for item in acces_items]

                card = Table(rows, colWidths=[8.1*cm])
                card.setStyle(TableStyle([
                    ("BACKGROUND", (0, 0), (-1, -1), p["bg"]),
                    ("BOX", (0, 0), (-1, -1), 1.5, p["border"]),
                    ("LEFTPADDING",  (0, 0), (-1, -1), 12),
                    ("RIGHTPADDING", (0, 0), (-1, -1), 12),
                    ("TOPPADDING",   (0, 0), (0, 0),  12),
                    ("BOTTOMPADDING",(0, -1),(-1,-1),  12),
                    ("TOPPADDING",   (0, 1), (-1, -1), 2),
                    ("BOTTOMPADDING",(0, 0), (-1, -2), 2),
                    ("ROUNDEDCORNERS", [6]),
                ]))
                cards_row.append(card)
            else:
                cards_row.append(Spacer(1, 1))

        t = Table([cards_row], colWidths=[8.1*cm, 8.9*cm])
        t.setStyle(TableStyle([
            ("VALIGN",        (0, 0), (-1, -1), "TOP"),
            ("LEFTPADDING",   (0, 0), (-1, -1), 0),
            ("RIGHTPADDING",  (0, 0), (-1, -1), 0),
            ("TOPPADDING",    (0, 0), (-1, -1), 0),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
        ]))
        story.append(t)

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 4 — NAVIGATION ET INTERFACE
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("4", "Navigation et Interface",
                                "Tableau de bord, menu latéral et en-tête", S))
    story.append(Spacer(1, 16))

    story.append(Paragraph("4.1 Tableau de Bord", S["h2"]))
    story.append(Paragraph(
        "Après connexion, vous accédez au <b>tableau de bord</b> qui constitue la page d'accueil "
        "principale. Il affiche tous les modules auxquels vous avez accès sous forme de tuiles colorées. "
        "Il suffit de cliquer sur une tuile pour accéder au module correspondant.",
        S["body"]
    ))
    story.append(Spacer(1, 8))
    story.append(info_box(
        "Le tableau de bord est <b>personnalisé</b> selon votre rôle. Un enseignant ne verra pas "
        "les modules financiers, et un parent ne verra que les modules pertinents pour suivre "
        "la scolarité de son enfant.",
        S, "tip"
    ))

    story.append(Spacer(1, 12))
    story.append(Paragraph("4.2 Menu de Navigation Latéral", S["h2"]))
    story.append(Paragraph(
        "Le menu de gauche (sidebar) est la principale interface de navigation. Il peut être "
        "<b>réduit ou élargi</b> en cliquant sur l'icône hamburger en haut. Chaque module est "
        "représenté par une icône et un libellé.",
        S["body"]
    ))
    story.append(Spacer(1, 8))

    nav_items = [
        ("🏠", "Tableau de bord",   "/",                         "Accueil de l'application"),
        ("📝", "Inscriptions",       "/inscriptions",             "Gestion des élèves et admissions"),
        ("📚", "Pédagogie",          "/pedagogie",                "Classes, matières, séances"),
        ("📊", "Évaluations",        "/evaluations",              "Notes, évaluations, bulletins"),
        ("🕐", "Présences",          "/presences",                "Appels, absences, badgeages"),
        ("💰", "Finances",           "/finances",                 "Factures, paiements, dépenses"),
        ("👥", "Personnel",          "/personnel",                "Enseignants, congés"),
        ("💬", "Communication",      "/communication",            "Messages, annonces"),
        ("🎉", "BDE",                "/bde",                      "Associations, événements"),
        ("📄", "Documents",          "/documents",                "Modèles et documents générés"),
        ("⭐", "Services Premium",   "/services-premium",         "Abonnements et services parents"),
        ("❓", "Support",            "/support",                  "Tickets, suggestions, aide"),
    ]

    nav_header = [
        Paragraph("Icône", ParagraphStyle("nh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Module", ParagraphStyle("nh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("URL", ParagraphStyle("nh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Description", ParagraphStyle("nh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
    ]
    nav_data = [nav_header]
    for icon, name, url, desc in nav_items:
        nav_data.append([
            Paragraph(icon, ParagraphStyle("nc", fontName="Helvetica", fontSize=11, textColor=GRAY_DARK, leading=14, alignment=TA_CENTER)),
            Paragraph(f"<b>{name}</b>", ParagraphStyle("nn", fontName="Helvetica-Bold", fontSize=9, textColor=GRAY_DARK, leading=12)),
            Paragraph(url, ParagraphStyle("nu", fontName="Helvetica-Oblique", fontSize=8.5, textColor=GREEN_DARK, leading=12)),
            Paragraph(desc, ParagraphStyle("nd", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=12)),
        ])

    nav_t = Table(nav_data, colWidths=[1.5*cm, 4*cm, 4.5*cm, 7*cm])
    nav_t.setStyle(TableStyle([
        ("BACKGROUND",   (0, 0), (-1, 0),  GREEN_DARK),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [WHITE, GRAY_LIGHT]),
        ("ALIGN",        (0, 0), (-1, -1), "LEFT"),
        ("VALIGN",       (0, 0), (-1, -1), "MIDDLE"),
        ("TOPPADDING",   (0, 0), (-1, -1), 7),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 7),
        ("LEFTPADDING",  (0, 0), (-1, -1), 8),
        ("RIGHTPADDING", (0, 0), (-1, -1), 8),
        ("LINEBELOW",    (0, 0), (-1, -1), 0.5, BORDER_COLOR),
        ("BOX",          (0, 0), (-1, -1), 1,   BORDER_COLOR),
        ("ROUNDEDCORNERS", [4]),
    ]))
    story.append(nav_t)

    story.append(Spacer(1, 12))
    story.append(Paragraph("4.3 En-tête et Menu Utilisateur", S["h2"]))
    story.append(Paragraph(
        "En haut de l'écran, l'en-tête contient plusieurs éléments utiles :",
        S["body"]
    ))
    story.append(Spacer(1, 6))

    header_items = [
        ("🌐", "Sélecteur de langue",    "Basculer entre Français et Anglais"),
        ("🔔", "Notifications",          "Alertes et messages non lus"),
        ("👤", "Menu utilisateur",        "Accès au profil, paramètres et déconnexion"),
        ("📶", "Indicateur réseau",       "Statut de la connexion (en ligne / hors ligne)"),
        ("🔄", "Statut de synchronisation","Avancement de la synchro des données hors ligne"),
    ]
    for icon, item, desc in header_items:
        story.append(Paragraph(
            f"    <b>{icon} {item}</b> — {desc}",
            ParagraphStyle("hi", fontName="Helvetica", fontSize=10, textColor=GRAY_MED,
                           leading=16, leftIndent=16)
        ))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 5 — MODULES FONCTIONNELS
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("5", "Modules Fonctionnels",
                                "Description complète de chaque module métier", S))
    story.append(Spacer(1, 16))

    modules_data = [
        {
            "num": "5.1", "icon": "📝", "title": "Module Inscriptions",
            "color": INDIGO_DARK, "bg": INDIGO_LIGHT, "border": HexColor("#a5b4fc"),
            "url": "/inscriptions",
            "description": (
                "Le module Inscriptions centralise tout le processus d'intégration des élèves. "
                "Il permet de gérer les dossiers d'admission depuis la candidature jusqu'à "
                "l'inscription définitive, et de maintenir à jour le registre des élèves."
            ),
            "submodules": [
                ("Élèves", "Liste complète des élèves inscrits avec leurs informations personnelles et scolaires"),
                ("Dossiers d'Admission", "Traitement des candidatures : réception, évaluation et validation des dossiers"),
                ("Inscriptions", "Gestion des inscriptions annuelles, renouvellements et transferts"),
                ("Années Scolaires", "Configuration des années scolaires, semestres et périodes d'évaluation"),
            ],
            "roles": ["super_admin", "directeur", "responsable_admissions", "censeur", "personnel_admin"],
        },
        {
            "num": "5.2", "icon": "📚", "title": "Module Pédagogie",
            "color": HexColor("#0f766e"), "bg": TEAL_LIGHT, "border": HexColor("#5eead4"),
            "url": "/pedagogie",
            "description": (
                "La pédagogie est le cœur de l'organisation scolaire. Ce module permet de "
                "structurer l'ensemble de l'offre éducative : niveaux, classes, matières, "
                "emplois du temps et suivi des contenus enseignés."
            ),
            "submodules": [
                ("Niveaux / Classes", "Définition des niveaux scolaires (6ème, 5ème, Terminale...)"),
                ("Classes", "Gestion des groupes d'élèves : effectifs, enseignants référents, salles"),
                ("Matières", "Catalogue des disciplines enseignées avec coefficients et volumes horaires"),
                ("Salles", "Inventaire des espaces pédagogiques et leur disponibilité"),
                ("Séances", "Emploi du temps et planning des cours"),
                ("Cahiers de Textes", "Suivi des contenus enseignés par séance"),
            ],
            "roles": ["super_admin", "directeur", "censeur", "chef_departement", "enseignant"],
        },
        {
            "num": "5.3", "icon": "📊", "title": "Module Évaluations",
            "color": VIOLET_DARK, "bg": VIOLET_LIGHT, "border": HexColor("#c4b5fd"),
            "url": "/evaluations",
            "description": (
                "Le module Évaluations couvre tout le cycle de l'évaluation scolaire : "
                "de la saisie des notes par l'enseignant jusqu'à l'édition des bulletins "
                "officiels transmis aux familles."
            ),
            "submodules": [
                ("Notes", "Saisie et consultation des résultats individuels par matière et période"),
                ("Évaluations", "Planification des contrôles, devoirs et examens"),
                ("Bulletins", "Génération automatique des bulletins de notes par période"),
            ],
            "roles": ["super_admin", "directeur", "censeur", "enseignant", "parent (lecture)", "élève (lecture)"],
        },
        {
            "num": "5.4", "icon": "🕐", "title": "Module Présences",
            "color": SKY_DARK, "bg": SKY_LIGHT, "border": HexColor("#7dd3fc"),
            "url": "/presences",
            "description": (
                "Le suivi de l'assiduité est critique pour la réussite scolaire. Ce module "
                "permet de réaliser les appels en classe, de gérer les justificatifs d'absence "
                "et d'intégrer les données de badgeage électronique."
            ),
            "submodules": [
                ("Appels", "Saisie des présences / absences par séance et par classe"),
                ("Absences", "Gestion des absences : signalement, justification, suivi cumulatif"),
                ("Badgeages", "Import et consultation des données de pointage électronique"),
            ],
            "roles": ["super_admin", "directeur", "censeur", "enseignant", "parent (lecture)"],
        },
        {
            "num": "5.5", "icon": "💰", "title": "Module Finances",
            "color": TEAL_DARK, "bg": TEAL_LIGHT, "border": HexColor("#5eead4"),
            "url": "/finances",
            "description": (
                "La gestion financière regroupe toutes les opérations comptables de "
                "l'établissement. Ce module est principalement destiné à l'intendant "
                "et à la direction pour le suivi budgétaire."
            ),
            "submodules": [
                ("Factures", "Édition des factures de scolarité, activités et autres prestations"),
                ("Paiements", "Enregistrement et suivi des paiements reçus"),
                ("Dépenses", "Saisie et catégorisation des dépenses de l'établissement"),
                ("Rémunérations", "Gestion des salaires et rémunérations du personnel enseignant"),
            ],
            "roles": ["super_admin", "directeur", "intendant", "fondateur (lecture)", "parent (ses factures)"],
        },
        {
            "num": "5.6", "icon": "👥", "title": "Module Personnel",
            "color": ORANGE_DARK, "bg": ORANGE_LIGHT, "border": HexColor("#fdba74"),
            "url": "/personnel",
            "description": (
                "Le module Personnel centralise la gestion des ressources humaines de "
                "l'établissement, notamment le suivi des enseignants et la gestion "
                "administrative des congés et absences du personnel."
            ),
            "submodules": [
                ("Enseignants", "Fiches des enseignants : informations personnelles, matières, disponibilités"),
                ("Demandes de Congés", "Soumission, validation et suivi des demandes de congés"),
            ],
            "roles": ["super_admin", "directeur", "admin_it", "censeur", "chef_departement", "enseignant (sa fiche)"],
        },
        {
            "num": "5.7", "icon": "💬", "title": "Module Communication",
            "color": PINK_DARK, "bg": PINK_LIGHT, "border": HexColor("#f9a8d4"),
            "url": "/communication",
            "description": (
                "La communication interne est facilitée par ce module qui regroupe "
                "la messagerie privée, les annonces officielles et les notifications "
                "automatiques de la plateforme."
            ),
            "submodules": [
                ("Messages", "Messagerie privée entre utilisateurs de l'établissement"),
                ("Notifications", "Centre de notifications système (absences, paiements, événements...)"),
                ("Annonces", "Publication d'annonces officielles visibles par tous ou par catégorie"),
            ],
            "roles": ["Tous les rôles (selon leur niveau d'accès)"],
        },
        {
            "num": "5.8", "icon": "🎉", "title": "Module BDE",
            "color": AMBER_DARK, "bg": AMBER_LIGHT, "border": HexColor("#fcd34d"),
            "url": "/bde",
            "description": (
                "Le Bureau Des Élèves dispose de son propre espace de gestion au sein "
                "de la plateforme. Ce module permet d'organiser la vie associative "
                "de l'établissement."
            ),
            "submodules": [
                ("Associations", "Création et gestion des clubs et associations d'élèves"),
                ("Événements", "Organisation des événements scolaires et parascolaires"),
                ("Membres BDE", "Liste des membres actifs et attribution des rôles"),
                ("Dépenses BDE", "Gestion du budget et des dépenses du bureau des élèves"),
            ],
            "roles": ["super_admin", "directeur", "responsable_bde", "élève (consultation)"],
        },
        {
            "num": "5.9", "icon": "📄", "title": "Module Documents",
            "color": PURPLE_DARK, "bg": PURPLE_LIGHT, "border": HexColor("#d8b4fe"),
            "url": "/documents",
            "description": (
                "La gestion documentaire permet de créer des modèles de documents officiels "
                "(attestations, certificats, bulletins personnalisés) et de les générer "
                "automatiquement pour les élèves ou le personnel."
            ),
            "submodules": [
                ("Modèles de Documents", "Bibliothèque de gabarits : attestations de scolarité, certificats..."),
                ("Documents Générés", "Historique des documents produits et téléchargeables"),
                ("Signatures", "Gestion des signatures électroniques pour les documents officiels"),
            ],
            "roles": ["super_admin", "directeur", "admin_it", "censeur", "intendant", "personnel_admin", "parent (ses docs)"],
        },
        {
            "num": "5.10", "icon": "⭐", "title": "Services Premium",
            "color": YELLOW_DARK, "bg": YELLOW_LIGHT, "border": HexColor("#fde68a"),
            "url": "/services-premium",
            "description": (
                "Les services premium offrent des fonctionnalités avancées et additionnelles "
                "pour les parents et l'établissement. Ils permettent une communication et "
                "un suivi enrichis de la scolarité."
            ),
            "submodules": [
                ("Services Parents", "Fonctionnalités avancées pour le suivi parental : alertes temps réel, rapports détaillés"),
                ("Souscriptions", "Gestion des abonnements aux services premium par famille"),
            ],
            "roles": ["super_admin", "directeur", "intendant", "parent (ses services)"],
        },
    ]

    for mod in modules_data:
        story.append(Paragraph(f"{mod['num']} {mod['title']}", S["h2"]))
        story.append(info_box(f"<b>URL :</b> <i>{mod['url']}</i>", S, "info"))
        story.append(Spacer(1, 6))
        story.append(Paragraph(mod["description"], S["body"]))
        story.append(Spacer(1, 6))
        story.append(Paragraph("<b>Sous-modules :</b>", S["body_bold"]))

        sub_data = [[
            Paragraph("Sous-module", ParagraphStyle("sh2", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
            Paragraph("Description", ParagraphStyle("sh2", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        ]]
        for sub_name, sub_desc in mod["submodules"]:
            sub_data.append([
                Paragraph(f"<b>{sub_name}</b>", ParagraphStyle("sc", fontName="Helvetica-Bold", fontSize=9, textColor=mod["color"], leading=12)),
                Paragraph(sub_desc, ParagraphStyle("sd2", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=12)),
            ])

        sub_t = Table(sub_data, colWidths=[4*cm, 13*cm])
        sub_t.setStyle(TableStyle([
            ("BACKGROUND",   (0, 0), (-1, 0), mod["color"]),
            ("ROWBACKGROUNDS", (0, 1), (-1, -1), [WHITE, mod["bg"]]),
            ("TOPPADDING",   (0, 0), (-1, -1), 7),
            ("BOTTOMPADDING",(0, 0), (-1, -1), 7),
            ("LEFTPADDING",  (0, 0), (-1, -1), 10),
            ("RIGHTPADDING", (0, 0), (-1, -1), 10),
            ("VALIGN",       (0, 0), (-1, -1), "MIDDLE"),
            ("LINEBELOW",    (0, 0), (-1, -1), 0.5, BORDER_COLOR),
            ("BOX",          (0, 0), (-1, -1), 1, mod["border"]),
            ("ROUNDEDCORNERS", [4]),
        ]))
        story.append(sub_t)
        story.append(Spacer(1, 6))
        roles_text = ", ".join(mod["roles"])
        story.append(Paragraph(f"<b>Rôles ayant accès :</b>  {roles_text}", ParagraphStyle(
            "rt", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=13
        )))
        story.append(Spacer(1, 14))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 6 — SUPPORT ET AIDE
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("6", "Support et Aide",
                                "Tickets, documentation et assistant IA", S))
    story.append(Spacer(1, 16))

    support_modules = [
        ("🎫", "Tickets de Support", "/support/tickets",
         "Signalez tout problème technique ou fonctionnel via un ticket. Décrivez votre problème, "
         "son niveau d'urgence et joignez des captures d'écran si nécessaire. "
         "Votre ticket sera traité par l'équipe d'administration."),
        ("💡", "Suggestions", "/support/suggestions",
         "Vous avez une idée d'amélioration pour la plateforme ? Soumettez vos suggestions ici. "
         "Les suggestions peuvent être votées par la communauté et les plus populaires "
         "seront intégrées dans les prochaines mises à jour."),
        ("📖", "Documentation / Aide", "/support/aide",
         "La base de connaissances regroupe des articles d'aide classés par module et par profil. "
         "Consultez les guides pas-à-pas, les FAQ et les tutoriels vidéo pour maîtriser "
         "toutes les fonctionnalités de la plateforme."),
        ("🤖", "Assistant IA", "/support/aide-ia",
         "L'assistant intelligent est disponible 24h/24 pour répondre à vos questions sur "
         "l'utilisation de la plateforme. Posez votre question en langage naturel et "
         "obtenez une réponse immédiate et contextuelle."),
    ]

    for icon, title, url, desc in support_modules:
        story.append(Paragraph(f"{icon}  <b>{title}</b>", S["h2"]))
        story.append(info_box(f"<b>URL :</b> <i>{url}</i>", S, "info"))
        story.append(Spacer(1, 4))
        story.append(Paragraph(desc, S["body"]))
        story.append(Spacer(1, 12))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 7 — PROFIL ET PARAMÈTRES
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("7", "Mon Profil et Paramètres",
                                "Personnalisation de votre espace", S))
    story.append(Spacer(1, 16))

    story.append(Paragraph("7.1 Modifier mon profil", S["h2"]))
    story.append(Paragraph(
        "Accessible via <b>Menu utilisateur → Mon Profil</b> (URL : <i>/profil</i>). "
        "Cette page vous permet de consulter et modifier vos informations personnelles.",
        S["body"]
    ))
    story.append(Spacer(1, 6))

    profil_fields = [
        ("Prénom", "Votre prénom tel qu'il apparaît dans l'application"),
        ("Nom de famille", "Votre nom de famille"),
        ("Adresse e-mail", "Email de connexion (non modifiable, contacter l'admin)"),
        ("Photo de profil", "Votre avatar s'affiche sous forme d'initiales si aucune photo n'est définie"),
        ("Dernier login", "Date et heure de votre dernière connexion (informatif)"),
    ]
    profil_data = [[
        Paragraph("Champ", ParagraphStyle("fh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        Paragraph("Description", ParagraphStyle("fh", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
    ]]
    for fname, fdesc in profil_fields:
        profil_data.append([
            Paragraph(f"<b>{fname}</b>", ParagraphStyle("fn", fontName="Helvetica-Bold", fontSize=9, textColor=GRAY_DARK, leading=12)),
            Paragraph(fdesc, ParagraphStyle("fd3", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=12)),
        ])
    profil_t = Table(profil_data, colWidths=[4.5*cm, 12.5*cm])
    profil_t.setStyle(TableStyle([
        ("BACKGROUND",    (0, 0), (-1, 0), GREEN_DARK),
        ("ROWBACKGROUNDS",(0, 1), (-1, -1), [WHITE, GRAY_LIGHT]),
        ("TOPPADDING",    (0, 0), (-1, -1), 7),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
        ("LEFTPADDING",   (0, 0), (-1, -1), 10),
        ("RIGHTPADDING",  (0, 0), (-1, -1), 10),
        ("VALIGN",        (0, 0), (-1, -1), "MIDDLE"),
        ("LINEBELOW",     (0, 0), (-1, -1), 0.5, BORDER_COLOR),
        ("BOX",           (0, 0), (-1, -1), 1, BORDER_COLOR),
        ("ROUNDEDCORNERS", [4]),
    ]))
    story.append(profil_t)

    story.append(Spacer(1, 12))
    story.append(Paragraph("7.2 Changer de mot de passe", S["h2"]))
    story.append(Paragraph(
        "Depuis la page Mon Profil, vous pouvez modifier votre mot de passe. Renseignez votre "
        "<b>mot de passe actuel</b>, puis saisissez et confirmez votre <b>nouveau mot de passe</b>.",
        S["body"]
    ))
    story.append(Spacer(1, 6))
    story.append(info_box(
        "Le mot de passe doit contenir au minimum <b>8 caractères</b> dont une majuscule, "
        "une minuscule, un chiffre et un caractère spécial. Exemple : <b>MonPasse@2025</b>",
        S, "warning"
    ))

    story.append(Spacer(1, 12))
    story.append(Paragraph("7.3 Préférences d'Affichage", S["h2"]))
    story.append(Paragraph(
        "Accessible via <b>Menu utilisateur → Paramètres</b> (URL : <i>/parametres</i>). "
        "Vous pouvez personnaliser votre expérience sur la plateforme :",
        S["body"]
    ))
    story.append(Spacer(1, 6))

    prefs = [
        ("🎨", "Thème",              "Choisissez entre Clair, Sombre ou Automatique (selon votre système)"),
        ("🌐", "Langue",             "Sélectionnez Français (par défaut) ou Anglais"),
        ("📧", "Notifications e-mail","Activez / désactivez les alertes par e-mail"),
        ("🔔", "Notifications push",  "Activez / désactivez les notifications dans le navigateur"),
    ]
    for icon, name, desc in prefs:
        story.append(Paragraph(
            f"    <b>{icon} {name}</b> — {desc}",
            ParagraphStyle("pp", fontName="Helvetica", fontSize=10, textColor=GRAY_MED,
                           leading=16, leftIndent=16)
        ))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # SECTION 8 — RÉFÉRENCE RAPIDE PAR TYPE D'UTILISATEUR
    # ═══════════════════════════════════════════════════════════════════
    story.append(section_header("8", "Référence Rapide par Type d'Utilisateur",
                                "Ce que vous pouvez faire selon votre profil", S))
    story.append(Spacer(1, 16))

    user_guides = [
        {
            "icon": "👑", "title": "Guide Administrateurs",
            "roles_label": "super_admin / admin_it",
            "color": GREEN_DARK, "bg": GREEN_LIGHT, "border": HexColor("#86efac"),
            "tasks": [
                ("Créer un compte utilisateur", "Menu → Paramètres → Utilisateurs → Nouveau"),
                ("Configurer l'établissement", "Menu → Paramètres → Configuration"),
                ("Consulter les logs d'audit", "Menu → Paramètres → Journaux d'audit"),
                ("Recharger le cache système", "Menu → Paramètres → Maintenance → Recharger le cache"),
                ("Gérer les rôles et permissions", "Menu → Paramètres → Rôles"),
            ]
        },
        {
            "icon": "🎓", "title": "Guide Direction",
            "roles_label": "directeur / censeur / fondateur",
            "color": INDIGO_DARK, "bg": INDIGO_LIGHT, "border": HexColor("#a5b4fc"),
            "tasks": [
                ("Consulter le tableau de bord global", "Page d'accueil (/)"),
                ("Valider une inscription", "Inscriptions → Dossiers d'Admission → Valider"),
                ("Publier une annonce", "Communication → Annonces → Nouvelle annonce"),
                ("Générer les bulletins", "Évaluations → Bulletins → Générer"),
                ("Suivre les absences", "Présences → Absences → Récapitulatif"),
                ("Voir les finances", "Finances → Tableau de bord financier"),
            ]
        },
        {
            "icon": "👨‍🏫", "title": "Guide Enseignants",
            "roles_label": "enseignant / chef_departement",
            "color": SKY_DARK, "bg": SKY_LIGHT, "border": HexColor("#7dd3fc"),
            "tasks": [
                ("Faire l'appel", "Présences → Appels → Sélectionner sa classe → Saisir"),
                ("Saisir les notes", "Évaluations → Notes → Ma classe → Saisir"),
                ("Mettre à jour le cahier de textes", "Pédagogie → Cahiers de Textes → Ajouter séance"),
                ("Déposer une demande de congé", "Personnel → Demandes de Congés → Nouvelle demande"),
                ("Consulter son emploi du temps", "Pédagogie → Séances → Mon planning"),
            ]
        },
        {
            "icon": "🗂️", "title": "Guide Personnel Administratif",
            "roles_label": "personnel_admin / intendant / responsable_admissions",
            "color": VIOLET_DARK, "bg": VIOLET_LIGHT, "border": HexColor("#c4b5fd"),
            "tasks": [
                ("Enregistrer un paiement", "Finances → Paiements → Nouveau paiement"),
                ("Générer une attestation", "Documents → Modèles → Sélectionner → Générer"),
                ("Traiter un dossier d'admission", "Inscriptions → Dossiers d'Admission"),
                ("Émettre une facture", "Finances → Factures → Nouvelle facture"),
            ]
        },
        {
            "icon": "👨‍👩‍👧", "title": "Guide Parents et Élèves",
            "roles_label": "parent / eleve",
            "color": ORANGE_DARK, "bg": ORANGE_LIGHT, "border": HexColor("#fdba74"),
            "tasks": [
                ("Consulter les notes", "Tableau de bord → Évaluations → Notes (lecture)"),
                ("Vérifier les absences", "Présences → Absences de mon enfant"),
                ("Télécharger un bulletin", "Évaluations → Bulletins → Télécharger"),
                ("Voir les factures", "Finances → Mes factures"),
                ("Contacter un enseignant", "Communication → Messages → Nouveau message"),
                ("Soumettre un ticket", "Support → Tickets → Nouveau ticket"),
            ]
        },
    ]

    for guide in user_guides:
        story.append(Paragraph(
            f"{guide['icon']}  {guide['title']}",
            ParagraphStyle("gt", fontName="Helvetica-Bold", fontSize=13,
                           textColor=guide["color"], leading=18, spaceBefore=12, spaceAfter=4)
        ))
        story.append(Paragraph(
            f"<i>Rôles concernés : {guide['roles_label']}</i>",
            ParagraphStyle("gr", fontName="Helvetica-Oblique", fontSize=9,
                           textColor=GRAY_MED, leading=13, spaceAfter=6)
        ))

        task_header = [
            Paragraph("Action", ParagraphStyle("th2", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
            Paragraph("Comment faire", ParagraphStyle("th2", fontName="Helvetica-Bold", fontSize=9, textColor=WHITE, leading=12)),
        ]
        task_data = [task_header]
        for action, how in guide["tasks"]:
            task_data.append([
                Paragraph(f"<b>{action}</b>", ParagraphStyle("ta", fontName="Helvetica-Bold", fontSize=9, textColor=GRAY_DARK, leading=12)),
                Paragraph(how, ParagraphStyle("th3", fontName="Helvetica", fontSize=9, textColor=GRAY_MED, leading=12)),
            ])

        task_t = Table(task_data, colWidths=[6.5*cm, 10.5*cm])
        task_t.setStyle(TableStyle([
            ("BACKGROUND",    (0, 0), (-1, 0), guide["color"]),
            ("ROWBACKGROUNDS",(0, 1), (-1, -1), [WHITE, guide["bg"]]),
            ("TOPPADDING",    (0, 0), (-1, -1), 7),
            ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
            ("LEFTPADDING",   (0, 0), (-1, -1), 10),
            ("RIGHTPADDING",  (0, 0), (-1, -1), 10),
            ("VALIGN",        (0, 0), (-1, -1), "MIDDLE"),
            ("LINEBELOW",     (0, 0), (-1, -1), 0.5, BORDER_COLOR),
            ("BOX",           (0, 0), (-1, -1), 1.5, guide["border"]),
            ("ROUNDEDCORNERS", [4]),
        ]))
        story.append(task_t)
        story.append(Spacer(1, 14))

    story.append(PageBreak())

    # ═══════════════════════════════════════════════════════════════════
    # PAGE FINALE
    # ═══════════════════════════════════════════════════════════════════
    story.append(Spacer(1, 60))
    final_inner = Table(
        [[Paragraph("🎓", ParagraphStyle("fl", fontName="Helvetica-Bold", fontSize=50,
                                          leading=60, textColor=WHITE, alignment=TA_CENTER))],
         [Spacer(1, 8)],
         [Paragraph("Kouroukan", ParagraphStyle("ft2", fontName="Helvetica-Bold",
                     fontSize=28, leading=34, textColor=WHITE, alignment=TA_CENTER))],
         [Paragraph("Plateforme de Gestion d'Établissement Scolaire", ParagraphStyle(
             "fs", fontName="Helvetica", fontSize=13, leading=18,
             textColor=HexColor("#bbf7d0"), alignment=TA_CENTER))],
         [Spacer(1, 20)],
         [HRFlowable(width="50%", thickness=1, color=HexColor("#86efac"), spaceAfter=16)],
         [Paragraph("Pour toute assistance supplémentaire,", ParagraphStyle(
             "fa", fontName="Helvetica", fontSize=11, leading=15,
             textColor=WHITE, alignment=TA_CENTER))],
         [Paragraph("rendez-vous dans Support → Aide ou Support → Assistant IA", ParagraphStyle(
             "fb", fontName="Helvetica-Bold", fontSize=11, leading=15,
             textColor=HexColor("#86efac"), alignment=TA_CENTER))],
         [Spacer(1, 30)],
         [Paragraph("Version 1.0 — Mars 2026", ParagraphStyle(
             "fv", fontName="Helvetica", fontSize=9, leading=12,
             textColor=HexColor("#86efac"), alignment=TA_CENTER))]
         ],
        colWidths=[W]
    )
    final_inner.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), GREEN_DARK),
        ("TOPPADDING",   (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING",(0, 0), (-1, -1), 4),
        ("LEFTPADDING",  (0, 0), (-1, -1), 40),
        ("RIGHTPADDING", (0, 0), (-1, -1), 40),
        ("TOPPADDING",   (0, 0), (0, 0), 50),
        ("BOTTOMPADDING",(0, -1),(-1,-1), 50),
        ("ALIGN", (0, 0), (-1, -1), "CENTER"),
        ("ROUNDEDCORNERS", [10]),
    ]))
    story.append(final_inner)

    # ─── Build ───────────────────────────────────────────────────────
    def on_page(canvas_obj, doc_obj):
        canvas_obj.saveState()
        w, h = A4
        # Footer
        canvas_obj.setFillColor(GRAY_MED)
        canvas_obj.setFont("Helvetica", 8)
        canvas_obj.drawString(2*cm, 1.2*cm, "Kouroukan — Guide d'Utilisation v1.0")
        canvas_obj.drawRightString(w - 2*cm, 1.2*cm, f"Page {doc_obj.page}")
        # Top accent line
        canvas_obj.setFillColor(GREEN_DARK)
        canvas_obj.rect(2*cm, h - 1.5*cm, w - 4*cm, 2, fill=1, stroke=0)
        canvas_obj.restoreState()

    doc.build(story, onFirstPage=on_page, onLaterPages=on_page)
    print(f"PDF generated: {output_path}")
    return output_path

if __name__ == "__main__":
    build_document()
