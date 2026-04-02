<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('communication'))

const messages = ref([
  { id: 1, expediteur: 'M. Barry', objet: 'Resultats du controle de Mathematiques', date: '2025-03-15', lu: true, extrait: 'Votre fille a obtenu 16/20 au dernier controle.' },
  { id: 2, expediteur: 'Administration', objet: 'Rappel: Reunion parents-professeurs', date: '2025-03-14', lu: true, extrait: 'La reunion aura lieu le 20 mars a 17h.' },
  { id: 3, expediteur: 'Mme Camara', objet: 'Comportement en classe', date: '2025-03-12', lu: false, extrait: 'Je souhaiterais vous rencontrer concernant...' },
  { id: 4, expediteur: 'Administration', objet: 'Paiement scolarite - Rappel', date: '2025-03-10', lu: true, extrait: 'Veuillez regulariser le paiement du 2eme trimestre.' },
  { id: 5, expediteur: 'M. Sylla', objet: 'Sortie pedagogique prevue', date: '2025-03-08', lu: true, extrait: 'Une sortie au musee est prevue pour la semaine prochaine.' },
])
</script>

<template>
  <ForfaitRequiredOverlay
    v-if="isLocked"
    user-type="famille"
  />
  <div
    v-else
    class="space-y-6"
  >
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.communication.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.communication.title') }}
      </h1>
    </div>

    <div class="space-y-3">
      <div
        v-for="msg in messages"
        :key="msg.id"
        :class="[
          'cursor-pointer rounded-xl border p-4 transition-colors hover:bg-gray-50 dark:hover:bg-gray-700/50',
          msg.lu ? 'border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800' : 'border-indigo-200 bg-indigo-50/50 dark:border-indigo-700 dark:bg-indigo-900/10',
        ]"
      >
        <div class="flex items-start justify-between">
          <div class="flex items-center gap-2">
            <div
              v-if="!msg.lu"
              class="h-2 w-2 rounded-full bg-indigo-500"
            />
            <p :class="['text-sm', msg.lu ? 'text-gray-600 dark:text-gray-400' : 'font-semibold text-gray-900 dark:text-white']">
              {{ msg.expediteur }}
            </p>
          </div>
          <span class="text-xs text-gray-400">{{ formatDate(msg.date) }}</span>
        </div>
        <p :class="['mt-1 text-sm', msg.lu ? 'text-gray-900 dark:text-white' : 'font-semibold text-gray-900 dark:text-white']">
          {{ msg.objet }}
        </p>
        <p class="mt-1 text-xs text-gray-500 line-clamp-1">
          {{ msg.extrait }}
        </p>
      </div>
    </div>
  </div>
</template>
