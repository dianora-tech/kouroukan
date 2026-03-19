<template>
  <div class="flex gap-2">
    <USelect
      v-model="selectedCountry"
      :items="countries"
      value-key="dial"
      class="w-36 shrink-0"
    >
      <template #leading="{ modelValue }">
        <UIcon v-if="currentFlag" :name="currentFlag" class="h-4 w-4 rounded-full" />
      </template>
    </USelect>

    <UInput
      v-model="localNumber"
      type="tel"
      :placeholder="currentPlaceholder"
      class="w-full"
      @input="onInput"
    />
  </div>
</template>

<script setup lang="ts">
interface Country {
  label: string
  dial: string
  flag: string
  placeholder: string
}

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const countries: Country[] = [
  { label: '🇬🇳 +224', dial: '+224', flag: 'i-circle-flags-gn', placeholder: '62 00 00 00' },
  { label: '🇸🇳 +221', dial: '+221', flag: 'i-circle-flags-sn', placeholder: '77 000 00 00' },
  { label: '🇲🇱 +223', dial: '+223', flag: 'i-circle-flags-ml', placeholder: '70 00 00 00' },
  { label: '🇨🇮 +225', dial: '+225', flag: 'i-circle-flags-ci', placeholder: '07 00 00 00 00' },
  { label: '🇸🇱 +232', dial: '+232', flag: 'i-circle-flags-sl', placeholder: '76 000 000' },
  { label: '🇱🇷 +231', dial: '+231', flag: 'i-circle-flags-lr', placeholder: '77 000 0000' },
  { label: '🇬🇼 +245', dial: '+245', flag: 'i-circle-flags-gw', placeholder: '955 00 00' },
]

const selectedCountry = ref('+224')
const localNumber = ref('')

const currentFlag = computed(() => {
  const country = countries.find(c => c.dial === selectedCountry.value)
  return country?.flag
})

const currentPlaceholder = computed(() => {
  const country = countries.find(c => c.dial === selectedCountry.value)
  return country?.placeholder || ''
})

// Parse initial value
onMounted(() => {
  if (props.modelValue) {
    const match = props.modelValue.match(/^(\+\d{3})\s?(.*)$/)
    if (match) {
      const dial = match[1]
      if (countries.some(c => c.dial === dial)) {
        selectedCountry.value = dial
      }
      localNumber.value = match[2]
    } else {
      localNumber.value = props.modelValue
    }
  }
})

function formatFullNumber(): string {
  const digits = localNumber.value.replace(/\D/g, '')
  if (!digits) return ''
  // Format with spaces (groups of 2)
  const formatted = digits.match(/.{1,2}/g)?.join(' ') || digits
  return `${selectedCountry.value} ${formatted}`
}

function onInput() {
  emit('update:modelValue', formatFullNumber())
}

watch(selectedCountry, () => {
  emit('update:modelValue', formatFullNumber())
})

watch(localNumber, () => {
  emit('update:modelValue', formatFullNumber())
})
</script>
