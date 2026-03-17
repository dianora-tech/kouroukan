export function usePortalSeo(options: {
  title: string
  description: string
  path?: string
  image?: string
  type?: 'website' | 'article'
}) {
  const siteUrl = 'https://www.kouroukan.gn'
  const defaultImage = `${siteUrl}/og-image.png`

  useSeoMeta({
    title: options.title,
    description: options.description,
    ogTitle: options.title,
    ogDescription: options.description,
    ogImage: options.image || defaultImage,
    ogUrl: options.path ? `${siteUrl}${options.path}` : siteUrl,
    ogType: options.type || 'website',
    ogSiteName: 'Kouroukan',
    ogLocale: 'fr_GN',
    twitterCard: 'summary_large_image',
    twitterTitle: options.title,
    twitterDescription: options.description,
    twitterImage: options.image || defaultImage,
  })
}
