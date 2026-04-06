# =============================================================================
# Kouroukan Frontend (Nuxt 4 SSR)
# =============================================================================
# Stage 1 : Build
FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm ci --prefer-offline --no-audit --legacy-peer-deps

COPY . .
RUN node scripts/patch-pinia.mjs
ENV NUXT_TYPECHECK=false
ENV NODE_OPTIONS="--max-old-space-size=1024"
ARG API_BASE_URL=http://gateway:5000
ENV API_BASE_URL=${API_BASE_URL}
ARG APP_VERSION=v1.0-dev
ENV NUXT_PUBLIC_APP_VERSION=${APP_VERSION}
RUN npm run build

# Stage 2 : Runtime
FROM node:20-alpine AS runtime

WORKDIR /app

RUN addgroup -g 1001 -S kouroukan && \
    adduser -S kouroukan -u 1001 -G kouroukan

COPY --from=build --chown=kouroukan:kouroukan /app/.output ./.output
COPY --from=build /app/node_modules/.package-lock.json /tmp/

# Install runtime dependencies required by Nuxt Image (ipx)
RUN npm install --no-save ipx 2>/dev/null || true

USER kouroukan

ENV NODE_ENV=production
ENV NUXT_SECURITY_ENABLED=false
ENV NITRO_PORT=3000
ENV NITRO_HOST=0.0.0.0

EXPOSE 3000

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://127.0.0.1:3000/ || exit 1

CMD ["node", ".output/server/index.mjs"]
