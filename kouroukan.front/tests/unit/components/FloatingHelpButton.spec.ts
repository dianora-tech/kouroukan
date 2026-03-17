import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'

// Stubs
const stubs = {
  UIcon: { template: '<span />', props: ['name'] },
  NuxtLink: {
    template: '<a :href="to" @click="$emit(\'click\')"><slot /></a>',
    props: ['to'],
  },
  Transition: { template: '<div><slot /></div>' },
}

// Global mocks config
const globalConfig = {
  stubs,
  mocks: {
    $t: (key: string) => key,
  },
}

// Override useRoute per-test via a mutable ref
let currentPath = '/dashboard'
vi.stubGlobal('useRoute', () => ({
  path: currentPath,
  params: {},
  query: {},
}))

async function getComponent() {
  // Clear module cache to pick up new useRoute value
  vi.resetModules()
  const mod = await import('~/shared/components/FloatingHelpButton.vue')
  return mod.default
}

describe('FloatingHelpButton.vue', () => {
  beforeEach(() => {
    currentPath = '/dashboard'
  })

  it('est visible par defaut sur les pages non exclues', async () => {
    const FloatingHelpButton = await getComponent()
    const wrapper = mount(FloatingHelpButton, {
      global: globalConfig,
    })

    expect(wrapper.find('button').exists()).toBe(true)
  })

  it('est cache sur la page /connexion', async () => {
    currentPath = '/connexion'
    const FloatingHelpButton = await getComponent()
    const wrapper = mount(FloatingHelpButton, {
      global: globalConfig,
    })

    // The entire div is v-if="isVisible", so button should not exist
    expect(wrapper.find('button').exists()).toBe(false)
  })

  it('affiche le panneau au clic', async () => {
    const FloatingHelpButton = await getComponent()
    const wrapper = mount(FloatingHelpButton, {
      global: globalConfig,
    })

    await wrapper.find('button').trigger('click')

    expect(wrapper.findAll('a')).toHaveLength(3)
  })

  it('contient les 3 liens corrects', async () => {
    const FloatingHelpButton = await getComponent()
    const wrapper = mount(FloatingHelpButton, {
      global: globalConfig,
    })

    await wrapper.find('button').trigger('click')

    const links = wrapper.findAll('a')
    const hrefs = links.map(l => l.attributes('href'))
    expect(hrefs).toContain('/support/aide-ia')
    expect(hrefs).toContain('/support/aide')
    expect(hrefs).toContain('/support/tickets')
  })

  it('ferme le panneau quand un lien est clique', async () => {
    const FloatingHelpButton = await getComponent()
    const wrapper = mount(FloatingHelpButton, {
      global: globalConfig,
    })

    await wrapper.find('button').trigger('click')
    expect(wrapper.findAll('a')).toHaveLength(3)

    await wrapper.find('a').trigger('click')

    // After click, panelOpen should be false, so no links
    expect(wrapper.findAll('a')).toHaveLength(0)
  })
})
