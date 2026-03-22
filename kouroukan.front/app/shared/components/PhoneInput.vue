<template>
  <div class="flex gap-2">
    <USelect
      v-model="selectedCountry"
      :items="countryOptions"
      class="w-36 shrink-0"
    />

    <UInput
      v-model="localNumber"
      type="tel"
      :placeholder="currentPlaceholder"
      class="w-full"
    />
  </div>
</template>

<script setup lang="ts">
interface Country {
  name: string
  dial: string
  flag: string
  placeholder: string
  maxDigits: number
}

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const countries: Country[] = [
  { name: 'GN', dial: '+224', flag: 'i-circle-flags-gn', placeholder: '629 81 79 70', maxDigits: 9 },
]

const countryOptions = countries.map(c => ({
  label: `${c.name} ${c.dial}`,
  value: c.dial,
  icon: c.flag,
}))

const selectedCountry = ref('+224')
const localNumber = ref('')

const currentCountry = computed(() =>
  countries.find(c => c.dial === selectedCountry.value) || countries[0],
)

const currentPlaceholder = computed(() => currentCountry.value.placeholder)

// Format digits as: XXX XX XX XX (3 + groups of 2)
function formatDigits(digits: string): string {
  if (!digits) return ''
  if (digits.length <= 3) return digits
  const result = digits.slice(0, 3)
  const rest = digits.slice(3)
  const groups = rest.match(/.{1,2}/g) || []
  return [result, ...groups].join(' ')
}

// Parse initial value
onMounted(() => {
  if (props.modelValue) {
    // Try to match a dial code prefix
    const match = props.modelValue.match(/^(\+\d{3})\s?(.*)$/)
    if (match) {
      const dial = match[1]
      if (countries.some(c => c.dial === dial)) {
        selectedCountry.value = dial
      }
      localNumber.value = formatDigits(match[2].replace(/\D/g, ''))
    }
    else {
      // Raw digits
      localNumber.value = formatDigits(props.modelValue.replace(/\D/g, ''))
    }
  }
})

function formatFullNumber(): string {
  const digits = localNumber.value.replace(/\D/g, '').slice(0, currentCountry.value.maxDigits)
  if (!digits) return ''
  // Emettre uniquement les chiffres bruts (sans indicatif ni espaces)
  return digits
}

watch(selectedCountry, () => {
  emit('update:modelValue', formatFullNumber())
})

watch(localNumber, (val) => {
  const digits = val.replace(/\D/g, '').slice(0, currentCountry.value.maxDigits)
  const formatted = formatDigits(digits)
  if (formatted !== val) {
    localNumber.value = formatted
  }
  emit('update:modelValue', formatFullNumber())
})
</script>
