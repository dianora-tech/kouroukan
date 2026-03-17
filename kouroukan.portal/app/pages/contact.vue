<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('contact.title')"
          :subtitle="$t('contact.subtitle')"
          centered
        />

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 mt-12">
          <!-- Form -->
          <SharedContactForm />

          <!-- Info -->
          <div class="space-y-8">
            <div>
              <h3 class="font-semibold text-lg text-gray-900 dark:text-white mb-6">{{ $t('contact.info.title') }}</h3>
              <div class="space-y-4">
                <div class="flex items-start gap-3">
                  <UIcon name="i-heroicons-map-pin" class="w-5 h-5 text-green-600 mt-0.5" />
                  <span class="text-gray-600 dark:text-gray-400">{{ $t('contact.info.address') }}</span>
                </div>
                <div class="flex items-start gap-3">
                  <UIcon name="i-heroicons-phone" class="w-5 h-5 text-green-600 mt-0.5" />
                  <a href="tel:+224XXXXXXXX" class="text-gray-600 dark:text-gray-400 hover:text-green-600">{{ $t('contact.info.phone') }}</a>
                </div>
                <div class="flex items-start gap-3">
                  <UIcon name="i-heroicons-envelope" class="w-5 h-5 text-green-600 mt-0.5" />
                  <a href="mailto:contact@kouroukan.gn" class="text-gray-600 dark:text-gray-400 hover:text-green-600">{{ $t('contact.info.email') }}</a>
                </div>
                <div class="flex items-start gap-3">
                  <UIcon name="i-heroicons-chat-bubble-left" class="w-5 h-5 text-green-600 mt-0.5" />
                  <a href="https://wa.me/224XXXXXXXX" target="_blank" rel="noopener" class="text-gray-600 dark:text-gray-400 hover:text-green-600">{{ $t('contact.info.whatsapp') }}</a>
                </div>
                <div class="flex items-start gap-3">
                  <UIcon name="i-heroicons-clock" class="w-5 h-5 text-green-600 mt-0.5" />
                  <span class="text-gray-600 dark:text-gray-400">{{ $t('contact.info.hours') }}</span>
                </div>
              </div>
            </div>

            <!-- Map placeholder -->
            <div class="rounded-xl bg-gray-100 dark:bg-gray-800 aspect-video flex items-center justify-center">
              <div class="text-center">
                <UIcon name="i-heroicons-map" class="w-12 h-12 text-gray-400 mx-auto" />
                <p class="mt-2 text-sm text-gray-500">Conakry, Guinee</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Contact' })
</script>
