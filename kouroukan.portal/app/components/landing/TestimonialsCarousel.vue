<template>
  <section class="py-20 bg-gray-50 dark:bg-gray-950">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('testimonials.title')"
        :subtitle="$t('testimonials.subtitle')"
        centered
      />

      <!-- Carousel -->
      <div class="relative overflow-hidden">
        <div
          ref="carouselRef"
          class="flex gap-6 overflow-x-auto snap-x snap-mandatory scrollbar-hide pb-4"
          style="scroll-behavior: smooth; -ms-overflow-style: none; scrollbar-width: none;"
        >
          <div
            v-for="testimonial in TESTIMONIALS"
            :key="testimonial.key"
            class="flex-shrink-0 w-80 sm:w-96 snap-center"
          >
            <div class="h-full p-6 rounded-xl bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 shadow-sm">
              <!-- Quote -->
              <UIcon name="i-heroicons-chat-bubble-bottom-center-text" class="w-8 h-8 text-green-200 mb-4" />
              <p class="text-gray-700 dark:text-gray-300 leading-relaxed italic">
                "{{ $t(`testimonials.items.${testimonial.key}.quote`) }}"
              </p>

              <!-- Author -->
              <div class="mt-6 flex items-center gap-3">
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

        <!-- Navigation dots -->
        <div class="flex justify-center gap-2 mt-6">
          <button
            v-for="(_, index) in TESTIMONIALS"
            :key="index"
            class="w-2.5 h-2.5 rounded-full transition-colors"
            :class="currentIndex === index ? 'bg-green-600' : 'bg-gray-300 dark:bg-gray-700'"
            @click="scrollTo(index)"
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { TESTIMONIALS } from '~/utils/constants'

const carouselRef = ref<HTMLElement | null>(null)
const currentIndex = ref(0)

function getInitials(name: string): string {
  return name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
}

function scrollTo(index: number) {
  if (!carouselRef.value) return
  const cards = carouselRef.value.children
  if (cards[index]) {
    const card = cards[index] as HTMLElement
    const container = carouselRef.value
    // Scroll horizontally within the container only — never scroll the page
    container.scrollTo({
      left: card.offsetLeft - container.offsetLeft,
      behavior: 'smooth',
    })
    currentIndex.value = index
  }
}

let autoplayInterval: ReturnType<typeof setInterval> | null = null

onMounted(() => {
  autoplayInterval = setInterval(() => {
    currentIndex.value = (currentIndex.value + 1) % TESTIMONIALS.length
    scrollTo(currentIndex.value)
  }, 5000)
})

onUnmounted(() => {
  if (autoplayInterval) clearInterval(autoplayInterval)
})
</script>

<style scoped>
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}
</style>
