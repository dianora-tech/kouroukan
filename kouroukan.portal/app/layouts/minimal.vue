<template>
  <div class="min-h-screen flex flex-col">
    <header class="border-b border-gray-200 dark:border-gray-800">
      <div class="max-w-4xl mx-auto px-4 py-4 flex items-center gap-3">
        <NuxtLink to="/" class="flex items-center gap-2 text-green-600 hover:text-green-700">
          <div class="w-8 h-8 bg-green-600 rounded-lg flex items-center justify-center">
            <span class="text-white font-bold text-sm">K</span>
          </div>
          <span class="font-semibold text-lg text-gray-900 dark:text-white">Kouroukan</span>
        </NuxtLink>
      </div>
    </header>
    <main class="flex-1 max-w-4xl mx-auto px-4 py-8 w-full">
      <slot />
    </main>
    <footer class="border-t border-gray-200 dark:border-gray-800 py-6">
      <div class="max-w-4xl mx-auto px-4 text-center text-sm text-gray-500">
        &copy; {{ new Date().getFullYear() }} Kouroukan. {{ $t('footer.copyright') }}
      </div>
    </footer>
  </div>
</template>
