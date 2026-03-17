<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('demo.title')"
          :subtitle="$t('demo.subtitle')"
          centered
        />

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 mt-12">
          <!-- Video facade -->
          <div>
            <div
              class="relative aspect-video rounded-xl bg-gray-900 overflow-hidden cursor-pointer group"
              @click="showVideo = true"
            >
              <div v-if="!showVideo" class="absolute inset-0 flex items-center justify-center">
                <div class="text-center">
                  <div class="w-20 h-20 rounded-full bg-white/20 backdrop-blur-sm flex items-center justify-center mx-auto group-hover:scale-110 transition-transform">
                    <UIcon name="i-heroicons-play" class="w-10 h-10 text-white ml-1" />
                  </div>
                  <p class="mt-4 text-white font-medium">Decouvrez Kouroukan en 3 minutes</p>
                </div>
              </div>
              <div v-else class="w-full h-full flex items-center justify-center text-white">
                <p class="text-sm">Video de demo (a venir)</p>
              </div>
            </div>
          </div>

          <!-- Demo request form -->
          <div>
            <div v-if="success" class="p-8 rounded-xl bg-green-50 dark:bg-green-900/20 text-center">
              <UIcon name="i-heroicons-check-circle" class="w-12 h-12 text-green-600 mx-auto mb-4" />
              <p class="text-green-800 dark:text-green-200 font-medium">{{ $t('demo.form.success') }}</p>
            </div>

            <UForm v-else :state="state" @submit="onSubmit" class="space-y-5">
              <UFormField :label="$t('demo.form.name')" name="name" required>
                <UInput v-model="state.name" class="w-full" />
              </UFormField>

              <UFormField :label="$t('demo.form.schoolName')" name="schoolName" required>
                <UInput v-model="state.schoolName" class="w-full" />
              </UFormField>

              <UFormField :label="$t('demo.form.role')" name="role" required>
                <USelect v-model="state.role" :items="roleOptions" class="w-full" />
              </UFormField>

              <UFormField :label="$t('demo.form.phone')" name="phone" required>
                <UInput v-model="state.phone" placeholder="+224 XX XX XX XX" class="w-full" />
              </UFormField>

              <UButton type="submit" color="primary" :loading="loading" block>
                {{ $t('demo.form.submit') }}
              </UButton>
            </UForm>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Demo' })

const { t } = useI18n()
const showVideo = ref(false)
const loading = ref(false)
const success = ref(false)

const state = reactive({
  name: '',
  schoolName: '',
  role: '',
  phone: ''
})

const roleOptions = computed(() => [
  { label: t('demo.form.roles.director'), value: 'director' },
  { label: t('demo.form.roles.admin'), value: 'admin' },
  { label: t('demo.form.roles.teacher'), value: 'teacher' },
  { label: t('demo.form.roles.parent'), value: 'parent' },
  { label: t('demo.form.roles.other'), value: 'other' }
])

async function onSubmit() {
  loading.value = true
  try {
    await $fetch('/api/demo/request', { method: 'POST', body: state })
  } catch {
    const stored = JSON.parse(localStorage.getItem('kouroukan_demo_requests') || '[]')
    stored.push({ ...state, date: new Date().toISOString() })
    localStorage.setItem('kouroukan_demo_requests', JSON.stringify(stored))
  }
  success.value = true
  loading.value = false
}
</script>
