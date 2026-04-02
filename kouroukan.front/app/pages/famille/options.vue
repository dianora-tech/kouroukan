<script setup lang="ts">
definePageMeta({ layout: 'default' })

const { t } = useI18n()

interface ServiceOption {
  id: number
  code: string
  nom: string
  description: string
  prixMensuel: number
  estDisponible: boolean
  icone: string
  abonne: boolean
  etablissement?: string
}

// Fake data — will be replaced by API
const options = ref<ServiceOption[]>([
  {
    id: 1,
    code: 'communication-etablissement',
    nom: 'Communication École ABC',
    description: 'Recevez les annonces, notifications et messages de l\'établissement École ABC',
    prixMensuel: 5000,
    estDisponible: true,
    icone: 'i-heroicons-chat-bubble-left-right',
    abonne: true,
    etablissement: 'École ABC',
  },
  {
    id: 2,
    code: 'communication-etablissement',
    nom: 'Communication Lycée National',
    description: 'Recevez les annonces, notifications et messages du Lycée National',
    prixMensuel: 5000,
    estDisponible: true,
    icone: 'i-heroicons-chat-bubble-left-right',
    abonne: false,
    etablissement: 'Lycée National',
  },
  {
    id: 3,
    code: 'traceur-gps',
    nom: 'Traceur GPS Enfant',
    description: 'Suivez en temps réel la position de votre enfant sur une carte. Nécessite un dispositif physique.',
    prixMensuel: 15000,
    estDisponible: false,
    icone: 'i-heroicons-map-pin',
    abonne: false,
  },
])

const saving = ref<number | null>(null)

function formatGNF(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

async function toggleAbonnement(option: ServiceOption) {
  saving.value = option.id
  // TODO: call API to subscribe/unsubscribe
  await new Promise(resolve => setTimeout(resolve, 500))
  option.abonne = !option.abonne
  saving.value = null
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('nav.famille.dashboard'), to: '/famille' },
          { label: 'Services & Options' },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        Services & Options
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        Gérez vos abonnements aux services supplémentaires
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
      <div
        v-for="option in options"
        :key="option.id"
        class="relative overflow-hidden rounded-xl border bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800"
        :class="option.abonne ? 'border-green-200 dark:border-green-800' : 'border-gray-200'"
      >
        <!-- Badge statut -->
        <div class="absolute right-4 top-4">
          <UBadge v-if="!option.estDisponible" color="neutral" variant="subtle" size="sm">
            Bientôt disponible
          </UBadge>
          <UBadge v-else-if="option.abonne" color="success" variant="subtle" size="sm">
            Abonné
          </UBadge>
        </div>

        <!-- Icône -->
        <div class="mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-green-50 dark:bg-green-900/20">
          <UIcon :name="option.icone" class="h-6 w-6 text-green-600" />
        </div>

        <!-- Contenu -->
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
          {{ option.nom }}
        </h3>
        <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
          {{ option.description }}
        </p>

        <!-- Établissement -->
        <div v-if="option.etablissement" class="mt-3">
          <UBadge color="info" variant="subtle" size="sm">
            <UIcon name="i-heroicons-building-office-2" class="mr-1 h-3 w-3" />
            {{ option.etablissement }}
          </UBadge>
        </div>

        <!-- Prix -->
        <div class="mt-4 text-xl font-bold text-gray-900 dark:text-white">
          {{ formatGNF(option.prixMensuel) }}
          <span class="text-sm font-normal text-gray-500">/mois</span>
        </div>

        <!-- Action -->
        <div class="mt-4">
          <UButton
            v-if="option.estDisponible"
            :color="option.abonne ? 'error' : 'primary'"
            :variant="option.abonne ? 'outline' : 'solid'"
            :loading="saving === option.id"
            block
            @click="toggleAbonnement(option)"
          >
            {{ option.abonne ? 'Se désabonner' : 'S\'abonner' }}
          </UButton>
          <UButton
            v-else
            color="neutral"
            variant="outline"
            disabled
            block
          >
            Non disponible
          </UButton>
        </div>
      </div>
    </div>
  </div>
</template>
