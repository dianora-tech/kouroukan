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
  { name: 'SN', dial: '+221', flag: 'i-circle-flags-sn', placeholder: '770 00 00 00', maxDigits: 9 },
  { name: 'ML', dial: '+223', flag: 'i-circle-flags-ml', placeholder: '700 00 00 00', maxDigits: 8 },
  { name: 'CI', dial: '+225', flag: 'i-circle-flags-ci', placeholder: '070 00 00 000', maxDigits: 10 },
  { name: 'SL', dial: '+232', flag: 'i-circle-flags-sl', placeholder: '760 00 000', maxDigits: 8 },
  { name: 'LR', dial: '+231', flag: 'i-circle-flags-lr', placeholder: '770 00 0000', maxDigits: 9 },
  { name: 'GW', dial: '+245', flag: 'i-circle-flags-gw', placeholder: '955 00 00', maxDigits: 7 },
]

const countryOptions = countries.map(c => ({
  label: `${c.name} ${c.dial}`,
  value: c.dial,
  icon: c.flag,
}))

const selectedCountry = ref('+224')
const localNumber = ref('')

const currentCountry = computed(() =>
  countries.find(c => c.dial === selectedCountry.value) || countries[0]
)

const currentPlaceholder = computed(() => currentCountry.value.placeholder)

// Format digits as: XXX XX XX XX (3 + groups of 2)
function formatDigits(digits: string): string {
  if (!digits) return ''
  if (digits.length <= 3) return digits
  let result = digits.slice(0, 3)
  let rest = digits.slice(3)
  const groups = rest.match(/.{1,2}/g) || []
  return [result, ...groups].join(' ')
}

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
  let digits = localNumber.value.replace(/\D/g, '')
  if (!digits) return ''
  digits = digits.slice(0, currentCountry.value.maxDigits)
  return `${selectedCountry.value} ${formatDigits(digits)}`
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
