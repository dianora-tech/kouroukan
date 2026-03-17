/**
 * Patches Pinia 2.3.x SSR bug where shouldHydrate() calls obj.hasOwnProperty()
 * on objects created with Object.create(null) that lack prototype methods.
 * Fix: use Object.prototype.hasOwnProperty.call() instead.
 * See: https://github.com/vuejs/pinia/issues/2822
 */
import { readFileSync, writeFileSync, existsSync } from 'fs'
import { join } from 'path'

const files = [
  'node_modules/pinia/dist/pinia.mjs',
  'node_modules/pinia/dist/pinia.cjs',
  'node_modules/pinia/dist/pinia.prod.cjs',
]

const bugPattern = /obj\.hasOwnProperty\(skipHydrateSymbol\)/g
const fix = 'Object.prototype.hasOwnProperty.call(obj, skipHydrateSymbol)'

let patched = 0
for (const file of files) {
  const fullPath = join(process.cwd(), file)
  if (!existsSync(fullPath)) continue
  const content = readFileSync(fullPath, 'utf-8')
  if (bugPattern.test(content)) {
    writeFileSync(fullPath, content.replace(bugPattern, fix))
    patched++
    console.log(`[patch-pinia] Patched ${file}`)
  }
}

if (patched === 0) {
  console.log('[patch-pinia] No files needed patching (already fixed or different version)')
} else {
  console.log(`[patch-pinia] Done — ${patched} file(s) patched`)
}
