import type { Page } from '@playwright/test'

/**
 * Login helper pour les tests E2E.
 * Utilise un mock API ou le vrai backend selon la configuration.
 */
export async function login(page: Page, role: 'directeur' | 'enseignant' | 'parent' | 'eleve' = 'directeur'): Promise<void> {
  const credentials: Record<string, { email: string; password: string }> = {
    directeur: { email: 'directeur@kouroukan.gn', password: 'Test1234!' },
    enseignant: { email: 'enseignant@kouroukan.gn', password: 'Test1234!' },
    parent: { email: 'parent@kouroukan.gn', password: 'Test1234!' },
    eleve: { email: 'eleve@kouroukan.gn', password: 'Test1234!' },
  }

  const { email, password } = credentials[role]

  await page.goto('/connexion')
  await page.fill('input[type="email"]', email)
  await page.fill('input[type="password"]', password)
  await page.click('button[type="submit"]')

  // Attendre la redirection vers le dashboard
  await page.waitForURL('**/dashboard**', { timeout: 10_000 })
}

/**
 * Login avec un utilisateur dont les CGU ne sont pas acceptees.
 */
export async function loginWithoutCgu(page: Page): Promise<void> {
  await page.goto('/connexion')
  await page.fill('input[type="email"]', 'new-user@kouroukan.gn')
  await page.fill('input[type="password"]', 'Test1234!')
  await page.click('button[type="submit"]')
}

/**
 * Logout helper.
 */
export async function logout(page: Page): Promise<void> {
  await page.click('[data-testid="user-menu"]')
  await page.click('[data-testid="logout-button"]')
  await page.waitForURL('**/connexion**')
}
