import { defineContentConfig, defineCollection, z } from '@nuxt/content'

export default defineContentConfig({
  collections: {
    blog: defineCollection({
      type: 'page',
      source: 'blog/**',
      schema: z.object({
        title: z.string(),
        description: z.string(),
        date: z.string(),
        category: z.string().optional(),
        author: z.string().optional(),
      }),
    }),
    aide: defineCollection({
      type: 'page',
      source: 'aide/**',
      schema: z.object({
        title: z.string(),
        description: z.string(),
        category: z.string().optional(),
        order: z.number().optional(),
      }),
    }),
    systemeEducatif: defineCollection({
      type: 'page',
      source: 'systeme-educatif/**',
      schema: z.object({
        title: z.string(),
        description: z.string(),
        slug: z.string().optional(),
        order: z.number().optional(),
      }),
    }),
  },
})
