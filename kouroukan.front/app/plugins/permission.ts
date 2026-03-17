import { vPermission } from '~/core/auth/directives/v-permission'

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.directive('permission', vPermission)
})
