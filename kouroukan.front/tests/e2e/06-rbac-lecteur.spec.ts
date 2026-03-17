import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 6 : RBAC — Boutons masques pour lecteur', () => {
  test('un parent ne voit pas les boutons create/delete sur inscriptions', async ({ page }) => {
    await login(page, 'parent')

    // Naviguer vers evaluations (parent a le droit de lire)
    await page.goto('/evaluations')

    // Les boutons de creation et suppression ne doivent pas etre visibles
    const createBtn = page.getByRole('button', { name: /cr[eé]er|nouveau/i })
    await expect(createBtn).toHaveCount(0)

    const deleteBtn = page.getByRole('button', { name: /supprimer/i })
    await expect(deleteBtn).toHaveCount(0)
  })

  test('un eleve ne voit pas les boutons de modification sur evaluations', async ({ page }) => {
    await login(page, 'eleve')

    await page.goto('/evaluations')

    const editBtn = page.getByRole('button', { name: /[eé]diter|modifier/i })
    await expect(editBtn).toHaveCount(0)
  })
})
