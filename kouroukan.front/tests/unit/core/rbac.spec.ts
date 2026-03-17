import { describe, it, expect } from 'vitest'
import { ROLES, PERMISSIONS } from '~/core/auth/rbac'
import type { RoleName, PermissionKey } from '~/core/auth/rbac'

describe('RBAC - Roles et Permissions', () => {
  describe('ROLES', () => {
    it('contient 13 roles', () => {
      expect(ROLES).toHaveLength(13)
    })

    it('contient les roles principaux', () => {
      expect(ROLES).toContain('super_admin')
      expect(ROLES).toContain('directeur')
      expect(ROLES).toContain('enseignant')
      expect(ROLES).toContain('parent')
      expect(ROLES).toContain('eleve')
    })
  })

  describe('PERMISSIONS', () => {
    // ─── inscriptions ───
    it('super_admin a inscriptions:create', () => {
      expect(PERMISSIONS['inscriptions:create']).toContain('super_admin')
    })

    it('directeur a inscriptions:create', () => {
      expect(PERMISSIONS['inscriptions:create']).toContain('directeur')
    })

    it('enseignant n\'a PAS inscriptions:create', () => {
      expect(PERMISSIONS['inscriptions:create']).not.toContain('enseignant')
    })

    it('parent n\'a PAS inscriptions:create', () => {
      expect(PERMISSIONS['inscriptions:create']).not.toContain('parent')
    })

    // ─── evaluations ───
    it('enseignant a evaluations:create', () => {
      expect(PERMISSIONS['evaluations:create']).toContain('enseignant')
    })

    it('parent a evaluations:read (consultation notes)', () => {
      expect(PERMISSIONS['evaluations:read']).toContain('parent')
    })

    it('eleve a evaluations:read', () => {
      expect(PERMISSIONS['evaluations:read']).toContain('eleve')
    })

    it('parent n\'a PAS evaluations:create', () => {
      expect(PERMISSIONS['evaluations:create']).not.toContain('parent')
    })

    // ─── finances ───
    it('intendant a finances:create', () => {
      expect(PERMISSIONS['finances:create']).toContain('intendant')
    })

    it('parent a finances:read (consultation factures)', () => {
      expect(PERMISSIONS['finances:read']).toContain('parent')
    })

    it('enseignant n\'a PAS finances:read', () => {
      expect(PERMISSIONS['finances:read']).not.toContain('enseignant')
    })

    // ─── support ───
    it('tous les roles ont support:read', () => {
      const supportRead = PERMISSIONS['support:read']
      expect(supportRead).toContain('parent')
      expect(supportRead).toContain('eleve')
      expect(supportRead).toContain('enseignant')
      expect(supportRead).toContain('directeur')
    })

    it('tous les roles sauf fondateur ont support:create', () => {
      const supportCreate = PERMISSIONS['support:create']
      expect(supportCreate).toContain('parent')
      expect(supportCreate).toContain('eleve')
      expect(supportCreate).not.toContain('fondateur')
    })

    it('seuls super_admin et admin_it ont support:delete', () => {
      const supportDelete = PERMISSIONS['support:delete']
      expect(supportDelete).toContain('super_admin')
      expect(supportDelete).toContain('admin_it')
      expect(supportDelete).not.toContain('directeur')
    })

    // ─── administration ───
    it('seuls super_admin et admin_it gerent les utilisateurs', () => {
      const usersManage = PERMISSIONS['users:manage']
      expect(usersManage).toEqual(['super_admin', 'admin_it'])
    })

    // ─── fondateur = lecture seule ───
    it('fondateur n\'a aucune permission create/update/delete', () => {
      const createUpdateDeletePerms = Object.entries(PERMISSIONS)
        .filter(([key]) => key.includes(':create') || key.includes(':update') || key.includes(':delete'))

      for (const [perm, roles] of createUpdateDeletePerms) {
        expect(roles, `${perm} ne devrait pas contenir fondateur`).not.toContain('fondateur')
      }
    })
  })
})
