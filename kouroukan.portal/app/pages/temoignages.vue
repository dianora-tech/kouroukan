<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('testimonials.allTestimonials')"
          :subtitle="$t('testimonials.subtitle')"
          centered
        />

        <!-- Filter by category -->
        <div class="flex flex-wrap justify-center gap-2 mb-12">
          <UButton
            v-for="cat in categories"
            :key="cat.key"
            size="sm"
            :variant="activeCategory === cat.key ? 'solid' : 'outline'"
            :color="activeCategory === cat.key ? 'primary' : 'neutral'"
            @click="activeCategory = cat.key"
          >
            {{ $t(`testimonials.categories.${cat.key}`) }}
          </UButton>
        </div>

        <!-- Testimonials grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="testimonial in filteredTestimonials"
            :key="testimonial.key"
            class="p-6 rounded-xl bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 shadow-sm"
          >
            <UIcon name="i-heroicons-chat-bubble-bottom-center-text" class="w-6 h-6 text-green-200 mb-3" />
            <p class="text-gray-700 dark:text-gray-300 leading-relaxed italic">
              "{{ $t(`testimonials.items.${testimonial.key}.quote`) }}"
            </p>
            <div class="mt-4 flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
                <span class="text-sm font-bold text-green-700 dark:text-green-400">
                  {{ getInitials($t(`testimonials.items.${testimonial.key}.name`)) }}
                </span>
              </div>
              <div>
                <div class="font-semibold text-sm text-gray-900 dark:text-white">
                  {{ $t(`testimonials.items.${testimonial.key}.name`) }}
                </div>
                <div class="text-xs text-gray-500">
                  {{ $t(`testimonials.items.${testimonial.key}.role`) }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
import { TESTIMONIALS } from '~/utils/constants'

useSeoMeta({ title: 'Temoignages' })

const activeCategory = ref('all')

const categories = [
  { key: 'all' },
  { key: 'directors' },
  { key: 'teachers' },
  { key: 'parents' },
  { key: 'staff' }
]

const filteredTestimonials = computed(() => {
  if (activeCategory.value === 'all') return TESTIMONIALS
  return TESTIMONIALS.filter(t => t.category === activeCategory.value)
})

function getInitials(name: string): string {
  return name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
}
</script>
