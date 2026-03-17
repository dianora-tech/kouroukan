export interface Module {
  slug: string
  name: string
  icon: string
  color: string
  description: string
  longDescription: string
}

export interface PricingPlan {
  key: string
  name: string
  price: string
  description: string
  features: string[]
  recommended?: boolean
}

export interface Testimonial {
  key: string
  name: string
  role: string
  quote: string
  category: 'directors' | 'teachers' | 'parents' | 'staff'
}

export interface FaqItem {
  question: string
  answer: string
  category: string
}

export interface RegistrationPayload {
  firstName: string
  lastName: string
  phoneNumber: string
  email?: string
  password: string
  modules: string[]
  region?: string
  prefecture?: string
  sousPrefecture?: string
  address?: string
}

export interface PortalStats {
  schools: number
  directors: number
  teachers: number
  parents: number
  students: number
  transactions: number
  documents: number
  isLive: boolean
}

export interface GeoRegion {
  name: string
  code: string
  prefectures?: GeoPrefecture[]
}

export interface GeoPrefecture {
  name: string
  code: string
}
