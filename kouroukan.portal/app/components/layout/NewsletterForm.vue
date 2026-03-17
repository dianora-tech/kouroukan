<template>
  <div class="space-y-2">
    <p class="text-sm font-medium text-white">{{ $t('footer.newsletter.title') }}</p>
    <form v-if="!success" class="flex gap-2" @submit.prevent="subscribe">
      <input
        v-model="email"
        type="email"
        :placeholder="$t('footer.newsletter.placeholder')"
        class="flex-1 px-3 py-2 text-sm bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-green-500"
        required
      />
      <UButton type="submit" color="primary" size="sm" :loading="loading">
        {{ $t('footer.newsletter.subscribe') }}
      </UButton>
    </form>
    <p v-if="success" class="text-sm text-green-400">{{ $t('footer.newsletter.success') }}</p>
    <p v-if="error" class="text-sm text-red-400">{{ $t('footer.newsletter.error') }}</p>
  </div>
</template>

<script setup lang="ts">
const email = ref('')
const loading = ref(false)
const success = ref(false)
const error = ref(false)

async function subscribe() {
  loading.value = true
  error.value = false
  try {
    await $fetch('/api/newsletter/subscribe', {
      method: 'POST',
      body: { email: email.value }
    })
    success.value = true
  } catch {
    // Store locally as fallback
    const stored = JSON.parse(localStorage.getItem('kouroukan_newsletter') || '[]')
    stored.push({ email: email.value, date: new Date().toISOString() })
    localStorage.setItem('kouroukan_newsletter', JSON.stringify(stored))
    success.value = true
  } finally {
    loading.value = false
  }
}
</script>
