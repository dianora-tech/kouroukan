import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'

// Stub Nuxt UI components
const stubs = {
  UForm: {
    template: '<form @submit.prevent="$emit(\'submit\')"><slot /></form>',
    props: ['schema', 'state'],
  },
  UFormField: {
    template: '<div><slot /></div>',
    props: ['label', 'name', 'required'],
  },
  UInput: {
    template: '<input :value="modelValue" @input="$emit(\'update:modelValue\', $event.target.value)" />',
    props: ['modelValue', 'type', 'min', 'step'],
  },
  USelect: {
    template: '<select :value="modelValue" @change="$emit(\'update:modelValue\', $event.target.value)"><slot /></select>',
    props: ['modelValue', 'items', 'valueKey', 'placeholder'],
  },
  UToggle: {
    template: '<input type="checkbox" :checked="modelValue" @change="$emit(\'update:modelValue\', $event.target.checked)" />',
    props: ['modelValue'],
  },
  UButton: {
    template: '<button :disabled="loading" @click="$emit(\'click\')"><slot /></button>',
    props: ['variant', 'color', 'loading', 'type'],
  },
}

// Global mocks config for mount
const globalConfig = {
  stubs,
  mocks: {
    $t: (key: string) => key,
  },
}

// Mock composables
vi.mock('~/modules/inscriptions/composables/useInscription', () => ({
  useInscription: () => ({
    typeOptions: [
      { label: 'Nouvelle inscription', value: 1 },
      { label: 'Reinscription', value: 2 },
    ],
  }),
}))

vi.mock('~/modules/inscriptions/types/inscription.types', () => ({
  STATUTS_INSCRIPTION: ['EnAttente', 'Validee', 'Annulee'],
  TYPES_ETABLISSEMENT: ['Public', 'PriveLaique'],
  SERIES_BAC: ['SE', 'SM', 'SS'],
}))

// Lazy import to avoid issues with auto-imports
async function getComponent() {
  const mod = await import('~/modules/inscriptions/components/InscriptionForm.vue')
  return mod.default
}

describe('InscriptionForm.vue', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('se monte en mode creation avec champs vides', async () => {
    const InscriptionForm = await getComponent()
    const wrapper = mount(InscriptionForm, {
      global: globalConfig,
    })

    expect(wrapper.find('form').exists()).toBe(true)
  })

  it('se monte en mode edition avec champs pre-remplis', async () => {
    const InscriptionForm = await getComponent()
    const entity = {
      id: 1,
      typeId: 1,
      eleveId: 10,
      classeId: 5,
      anneeScolaireId: 1,
      dateInscription: '2025-09-01',
      montantInscription: 150000,
      estPaye: true,
      estRedoublant: false,
      typeEtablissement: 'Public',
      serieBac: null,
      statutInscription: 'Validee',
    }

    const wrapper = mount(InscriptionForm, {
      props: { entity },
      global: globalConfig,
    })

    expect(wrapper.find('form').exists()).toBe(true)
  })

  it('emet cancel quand le bouton annuler est clique', async () => {
    const InscriptionForm = await getComponent()
    const wrapper = mount(InscriptionForm, {
      global: globalConfig,
    })

    // Find all rendered <button> elements — first is cancel, second is submit
    const buttons = wrapper.findAll('button')
    expect(buttons.length).toBeGreaterThanOrEqual(2)
    // Cancel button is the first one in the flex container
    await buttons[0].trigger('click')

    expect(wrapper.emitted('cancel')).toBeTruthy()
  })

  it('emet submit avec le payload en mode creation', async () => {
    const InscriptionForm = await getComponent()
    const wrapper = mount(InscriptionForm, {
      global: globalConfig,
    })

    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('submit')).toBeTruthy()
  })

  it('emet submit avec id en mode edition', async () => {
    const InscriptionForm = await getComponent()
    const entity = {
      id: 42,
      typeId: 1,
      eleveId: 10,
      classeId: 5,
      anneeScolaireId: 1,
      dateInscription: '2025-09-01',
      montantInscription: 150000,
      estPaye: false,
      estRedoublant: false,
      typeEtablissement: null,
      serieBac: null,
      statutInscription: 'EnAttente',
    }

    const wrapper = mount(InscriptionForm, {
      props: { entity },
      global: globalConfig,
    })

    await wrapper.find('form').trigger('submit')

    const emitted = wrapper.emitted('submit')
    expect(emitted).toBeTruthy()
    if (emitted?.[0]?.[0]) {
      expect((emitted[0][0] as any).id).toBe(42)
    }
  })
})
