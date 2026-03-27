<template>
  <section class="relative min-h-screen flex items-center bg-gradient-to-br from-green-700 via-green-600 to-green-800 overflow-hidden">
    <!-- Background SVG illustration -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <!-- Formes decoratives -->
      <div class="absolute top-20 left-10 w-72 h-72 bg-white rounded-full blur-3xl opacity-10" />
      <div class="absolute bottom-20 right-10 w-96 h-96 bg-yellow-400 rounded-full blur-3xl opacity-10" />

      <!-- Illustration communaute educative -->
      <svg class="absolute right-0 top-1/2 -translate-y-1/2 w-[600px] h-[600px] lg:w-[700px] lg:h-[700px] opacity-[0.12] lg:opacity-[0.15]" viewBox="0 0 500 500" fill="none" xmlns="http://www.w3.org/2000/svg">
        <!-- Cercle central (plateforme/securite) -->
        <circle cx="250" cy="250" r="120" stroke="white" stroke-width="2" stroke-dasharray="8 4" />
        <circle cx="250" cy="250" r="80" stroke="white" stroke-width="1.5" opacity="0.6" />
        <!-- Bouclier securite au centre -->
        <path d="M250 200 L250 200 C250 200 290 210 290 240 C290 270 250 300 250 300 C250 300 210 270 210 240 C210 210 250 200 250 200Z" fill="white" opacity="0.3" />
        <path d="M238 245 L248 255 L265 235" stroke="white" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />

        <!-- Enseignant (haut gauche) -->
        <g transform="translate(120, 100)">
          <circle cx="30" cy="20" r="18" fill="white" opacity="0.4" />
          <rect x="15" y="42" width="30" height="35" rx="10" fill="white" opacity="0.3" />
          <!-- Livre -->
          <rect x="8" y="55" width="16" height="12" rx="2" fill="white" opacity="0.5" />
        </g>
        <!-- Ligne connexion enseignant -> centre -->
        <line x1="150" y1="140" x2="220" y2="220" stroke="white" stroke-width="1.5" opacity="0.3" stroke-dasharray="4 3" />
        <circle cx="185" cy="180" r="3" fill="white" opacity="0.5" />

        <!-- Parent (bas gauche) -->
        <g transform="translate(100, 340)">
          <circle cx="30" cy="20" r="18" fill="white" opacity="0.4" />
          <rect x="15" y="42" width="30" height="35" rx="10" fill="white" opacity="0.3" />
          <!-- Coeur protection -->
          <path d="M25 55 C25 50 30 48 33 52 C36 48 41 50 41 55 C41 62 33 67 33 67 C33 67 25 62 25 55Z" fill="white" opacity="0.5" />
        </g>
        <!-- Ligne connexion parent -> centre -->
        <line x1="130" y1="370" x2="210" y2="290" stroke="white" stroke-width="1.5" opacity="0.3" stroke-dasharray="4 3" />
        <circle cx="170" cy="330" r="3" fill="white" opacity="0.5" />

        <!-- Eleve (droite) -->
        <g transform="translate(350, 200)">
          <circle cx="25" cy="18" r="16" fill="white" opacity="0.4" />
          <rect x="12" y="38" width="26" height="30" rx="8" fill="white" opacity="0.3" />
          <!-- Sac d'ecole -->
          <rect x="30" y="42" width="12" height="16" rx="3" fill="white" opacity="0.5" />
        </g>
        <!-- Ligne connexion eleve -> centre -->
        <line x1="365" y1="240" x2="300" y2="250" stroke="white" stroke-width="1.5" opacity="0.3" stroke-dasharray="4 3" />
        <circle cx="332" cy="245" r="3" fill="white" opacity="0.5" />

        <!-- Petit eleve (haut droite) -->
        <g transform="translate(320, 80)">
          <circle cx="20" cy="15" r="13" fill="white" opacity="0.3" />
          <rect x="10" y="32" width="20" height="24" rx="7" fill="white" opacity="0.2" />
        </g>
        <line x1="340" y1="120" x2="290" y2="200" stroke="white" stroke-width="1" opacity="0.2" stroke-dasharray="3 3" />

        <!-- Parent 2 (bas droite) -->
        <g transform="translate(340, 360)">
          <circle cx="22" cy="16" r="14" fill="white" opacity="0.3" />
          <rect x="10" y="34" width="24" height="28" rx="8" fill="white" opacity="0.2" />
        </g>
        <line x1="362" y1="370" x2="300" y2="300" stroke="white" stroke-width="1" opacity="0.2" stroke-dasharray="3 3" />

        <!-- Points de connexion animes -->
        <circle cx="250" cy="170" r="4" fill="white" opacity="0.6">
          <animate attributeName="opacity" values="0.6;0.2;0.6" dur="3s" repeatCount="indefinite" />
        </circle>
        <circle cx="300" cy="280" r="4" fill="white" opacity="0.6">
          <animate attributeName="opacity" values="0.2;0.6;0.2" dur="3s" repeatCount="indefinite" />
        </circle>
        <circle cx="200" cy="300" r="4" fill="white" opacity="0.6">
          <animate attributeName="opacity" values="0.6;0.2;0.6" dur="2.5s" repeatCount="indefinite" />
        </circle>

        <!-- Orbite exterieure -->
        <circle cx="250" cy="250" r="180" stroke="white" stroke-width="0.8" opacity="0.15" stroke-dasharray="12 8" />
      </svg>
    </div>

    <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-32 sm:py-40">
      <div class="max-w-3xl">
        <!-- Badge -->
        <div class="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/10 backdrop-blur-sm border border-white/20 mb-8">
          <span class="inline-block w-5 h-3.5 rounded-sm overflow-hidden">
            <div class="flex h-full">
              <div class="w-1/3 bg-red-600"></div>
              <div class="w-1/3 bg-yellow-400"></div>
              <div class="w-1/3 bg-green-600"></div>
            </div>
          </span>
          <span class="text-sm text-white font-medium">{{ $t('hero.badge') }}</span>
        </div>

        <!-- Title -->
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-white leading-tight">
          {{ $t('hero.title') }}
        </h1>

        <!-- Subtitle -->
        <p class="mt-6 text-lg sm:text-xl text-white/80 max-w-2xl leading-relaxed">
          {{ $t('hero.subtitle') }}
        </p>

        <!-- CTAs -->
        <div class="mt-10 flex flex-col sm:flex-row gap-4">
          <UButton
            :to="localePath('/inscription')"
            size="xl"
            color="white"
            class="font-semibold"
          >
            <UIcon name="i-heroicons-rocket-launch" class="w-5 h-5" />
            {{ $t('hero.cta') }}
          </UButton>
          <UButton
            :to="localePath('/demo')"
            size="xl"
            variant="outline"
            class="font-semibold text-white border-white/30 hover:bg-white/10"
          >
            <UIcon name="i-heroicons-play" class="w-5 h-5" />
            {{ $t('hero.ctaDemo') }}
          </UButton>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
</script>
