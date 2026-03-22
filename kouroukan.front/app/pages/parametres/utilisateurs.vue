<script setup lang="ts">
import { useUsersStore } from '~/core/stores/users.store'
import { useAuthStore } from '~/core/stores/auth.store'

const { t } = useI18n()
const toast = useToast()
const users = useUsersStore()
const auth = useAuthStore()

const showCreateModal = ref(false)
const showPasswordModal = ref(false)
const tempPassword = ref('')
const createdUserName = ref('')

const searchQuery = ref('')
const searchMode = ref(false)

const form = reactive({
  firstName: '',
  lastName: '',
  phoneNumber: '',
  email: '',
  role: 'enseignant',
  existingUserId: undefined as number | undefined,
})

const availableRoles = [
  { label: t('roles.fondateur'), value: 'fondateur' },
  { label: t('roles.admin_it'), value: 'admin_it' },
  { label: t('roles.censeur'), value: 'censeur' },
  { label: t('roles.intendant'), value: 'intendant' },
  { label: t('roles.responsable_admissions'), value: 'responsable_admissions' },
  { label: t('roles.chef_departement'), value: 'chef_departement' },
  { label: t('roles.enseignant'), value: 'enseignant' },
  { label: t('roles.personnel_admin'), value: 'personnel_admin' },
  { label: t('roles.responsable_bde'), value: 'responsable_bde' },
]

const isFondateurRole = computed(() => form.role === 'fondateur')

function resetForm() {
  form.firstName = ''
  form.lastName = ''
  form.phoneNumber = ''
  form.email = ''
  form.role = 'enseignant'
  form.existingUserId = undefined
  searchQuery.value = ''
  searchMode.value = false
  users.searchResult = null
}

async function handleSearch() {
  if (!searchQuery.value) return
  await users.searchUser(searchQuery.value)
}

function linkExistingUser() {
  if (!users.searchResult) return
  form.existingUserId = users.searchResult.id
  form.firstName = users.searchResult.firstName
  form.lastName = users.searchResult.lastName
}

async function handleCreate() {
  if (!form.firstName || !form.lastName) {
    if (!form.existingUserId && (!form.phoneNumber || !form.firstName || !form.lastName)) {
      toast.add({ title: t('validation.required'), color: 'error' })
      return
    }
  }

  try {
    const result = await users.createUser({
      firstName: form.firstName,
      lastName: form.lastName,
      phoneNumber: form.phoneNumber,
      email: form.email || undefined,
      role: form.role,
      existingUserId: form.existingUserId,
    })

    showCreateModal.value = false

    if (result.temporaryPassword) {
      createdUserName.value = `${form.firstName} ${form.lastName}`
      tempPassword.value = result.temporaryPassword
      showPasswordModal.value = true
    }
    else {
      toast.add({ title: t('users.linked'), color: 'success' })
    }

    resetForm()
  }
  catch {
    // Le toast d'erreur est affiche automatiquement par apiClient
  }
}

async function handleDelete(userId: number) {
  await users.deleteUser(userId)
  toast.add({ title: t('users.deleted'), color: 'success' })
}

function copyPassword() {
  navigator.clipboard.writeText(tempPassword.value)
  toast.add({ title: t('users.passwordCopied'), color: 'success' })
}

onMounted(() => {
  users.fetchUsers()
})
</script>

<template>
  <div>
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t('users.title') }}</h1>
        <p class="mt-1 text-sm text-gray-500">{{ $t('users.subtitle') }}</p>
      </div>
      <UButton
        v-if="auth.hasPermission('users:manage')"
        color="primary"
        icon="i-heroicons-plus"
        @click="showCreateModal = true; resetForm()"
      >
        {{ $t('users.add') }}
      </UButton>
    </div>

    <!-- Users Table -->
    <div class="rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
      <div v-if="users.loading" class="flex items-center justify-center p-12">
        <UIcon name="i-heroicons-arrow-path" class="h-6 w-6 animate-spin text-gray-400" />
      </div>

      <div v-else-if="users.items.length === 0" class="p-12 text-center text-gray-500">
        {{ $t('users.empty') }}
      </div>

      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-gray-200 dark:border-gray-700">
            <th class="px-4 py-3 text-left text-xs font-medium uppercase text-gray-500">{{ $t('users.name') }}</th>
            <th class="px-4 py-3 text-left text-xs font-medium uppercase text-gray-500">{{ $t('users.role') }}</th>
            <th class="px-4 py-3 text-left text-xs font-medium uppercase text-gray-500">{{ $t('users.phone') }}</th>
            <th class="px-4 py-3 text-left text-xs font-medium uppercase text-gray-500">{{ $t('users.email') }}</th>
            <th class="px-4 py-3 text-right text-xs font-medium uppercase text-gray-500">{{ $t('users.actions') }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users.items" :key="user.id" class="border-b border-gray-100 dark:border-gray-700/50">
            <td class="px-4 py-3 text-sm font-medium text-gray-900 dark:text-white">
              {{ user.firstName }} {{ user.lastName }}
            </td>
            <td class="px-4 py-3">
              <UBadge :color="user.role === 'directeur' ? 'primary' : 'neutral'" variant="subtle" size="sm">
                {{ $t(`roles.${user.role}`) }}
              </UBadge>
            </td>
            <td class="px-4 py-3 text-sm text-gray-600 dark:text-gray-400">{{ user.phoneNumber }}</td>
            <td class="px-4 py-3 text-sm text-gray-600 dark:text-gray-400">{{ user.email }}</td>
            <td class="px-4 py-3 text-right">
              <UButton
                v-if="user.role !== 'directeur'"
                variant="ghost"
                color="error"
                size="xs"
                icon="i-heroicons-trash"
                @click="handleDelete(user.id)"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create Modal -->
    <UModal v-model:open="showCreateModal">
      <template #content>
        <div class="p-6">
          <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('users.addTitle') }}</h2>

          <div class="space-y-4">
            <!-- Role -->
            <UFormField :label="$t('users.roleLabel')">
              <USelect v-model="form.role" :items="availableRoles" class="w-full" />
            </UFormField>

            <!-- Fondateur Search -->
            <template v-if="isFondateurRole">
              <UFormField :label="$t('users.searchFondateur')">
                <div class="flex gap-2">
                  <UInput
                    v-model="searchQuery"
                    :placeholder="$t('users.searchPlaceholder')"
                    class="flex-1"
                  />
                  <UButton
                    :loading="users.searching"
                    @click="handleSearch"
                  >
                    {{ $t('users.search') }}
                  </UButton>
                </div>
              </UFormField>

              <div v-if="users.searchResult" class="rounded-lg border border-green-200 bg-green-50 p-3 dark:border-green-800 dark:bg-green-900/20">
                <p class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ users.searchResult.firstName }} {{ users.searchResult.lastName }}
                </p>
                <UButton size="xs" color="primary" class="mt-2" @click="linkExistingUser">
                  {{ $t('users.linkUser') }}
                </UButton>
              </div>

              <div v-if="users.searchResult === null && searchQuery && !users.searching" class="text-sm text-gray-500">
                {{ $t('users.notFound') }}
              </div>
            </template>

            <!-- User fields (hidden if linking existing) -->
            <template v-if="!form.existingUserId">
              <div class="grid grid-cols-2 gap-4">
                <UFormField :label="$t('users.firstName')">
                  <UInput v-model="form.firstName" class="w-full" />
                </UFormField>
                <UFormField :label="$t('users.lastName')">
                  <UInput v-model="form.lastName" class="w-full" />
                </UFormField>
              </div>
              <UFormField :label="$t('users.phone')">
                <PhoneInput v-model="form.phoneNumber" />
              </UFormField>
              <UFormField :label="$t('users.email')">
                <UInput v-model="form.email" type="email" class="w-full" />
              </UFormField>
            </template>

            <div v-if="form.existingUserId" class="rounded-lg bg-blue-50 p-3 text-sm text-blue-700 dark:bg-blue-900/20 dark:text-blue-300">
              {{ $t('users.linkingExisting', { name: `${form.firstName} ${form.lastName}` }) }}
            </div>
          </div>

          <div class="mt-6 flex justify-end gap-3">
            <UButton variant="outline" @click="showCreateModal = false">{{ $t('common.cancel') }}</UButton>
            <UButton color="primary" :loading="users.saving" @click="handleCreate">{{ $t('users.create') }}</UButton>
          </div>
        </div>
      </template>
    </UModal>

    <!-- Password Modal -->
    <UModal v-model:open="showPasswordModal">
      <template #content>
        <div class="p-6 text-center">
          <div class="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/30">
            <UIcon name="i-heroicons-check" class="h-6 w-6 text-green-600" />
          </div>
          <h2 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('users.created') }}</h2>
          <p class="mb-4 text-sm text-gray-500">{{ $t('users.passwordInfo', { name: createdUserName }) }}</p>

          <div class="mx-auto flex max-w-xs items-center gap-2 rounded-lg bg-gray-100 px-4 py-3 font-mono text-lg dark:bg-gray-700">
            <span class="flex-1">{{ tempPassword }}</span>
            <UButton variant="ghost" size="xs" icon="i-heroicons-clipboard" @click="copyPassword" />
          </div>

          <p class="mt-3 text-xs text-red-500">{{ $t('users.passwordWarning') }}</p>

          <UButton color="primary" class="mt-6" @click="showPasswordModal = false">{{ $t('common.close') }}</UButton>
        </div>
      </template>
    </UModal>
  </div>
</template>
