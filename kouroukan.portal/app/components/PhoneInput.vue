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
}

const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const countries: Country[] = [
  { name: 'GN', dial: '+224', flag: 'i-circle-flags-gn', placeholder: '62 00 00 00' },
  { name: 'SN', dial: '+221', flag: 'i-circle-flags-sn', placeholder: '77 000 00 00' },
  { name: 'ML', dial: '+223', flag: 'i-circle-flags-ml', placeholder: '70 00 00 00' },
  { name: 'CI', dial: '+225', flag: 'i-circle-flags-ci', placeholder: '07 00 00 00 00' },
  { name: 'SL', dial: '+232', flag: 'i-circle-flags-sl', placeholder: '76 000 000' },
  { name: 'LR', dial: '+231', flag: 'i-circle-flags-lr', placeholder: '77 000 0000' },
  { name: 'GW', dial: '+245', flag: 'i-circle-flags-gw', placeholder: '955 00 00' },
]

const countryOptions = countries.map(c => ({
  label: `${c.name} ${c.dial}`,
  value: c.dial,
  icon: c.flag,
}))

const selectedCountry = ref('+224')
const localNumber = ref('')

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
  const formatted = digits.match(/.{1,2}/g)?.join(' ') || digits
  return `${selectedCountry.value} ${formatted}`
}

watch(selectedCountry, () => {
  emit('update:modelValue', formatFullNumber())
})

watch(localNumber, () => {
  emit('update:modelValue', formatFullNumber())
})
</script>
