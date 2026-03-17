import type { Directive } from 'vue'
import { useAuthStore } from '~/core/stores/auth.store'
import type { PermissionKey } from '~/core/auth/rbac'

export const vPermission: Directive<HTMLElement, PermissionKey> = {
  mounted(el, binding) {
    const auth = useAuthStore()
    const permission = binding.value

    if (!permission) return

    if (!auth.hasPermission(permission)) {
      el.style.display = 'none'
    }
  },

  updated(el, binding) {
    const auth = useAuthStore()
    const permission = binding.value

    if (!permission) return

    el.style.display = auth.hasPermission(permission) ? '' : 'none'
  },
}
