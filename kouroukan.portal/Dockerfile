# =============================================================================
# Kouroukan Portal (Nuxt 4 SSG)
# =============================================================================
# Stage 1 : Generate static
FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm ci --prefer-offline --no-audit

COPY . .
RUN npm run generate

# Stage 2 : Serve with Nginx
FROM nginx:alpine AS runtime

RUN rm /etc/nginx/conf.d/default.conf

COPY --from=build /app/.output/public /usr/share/nginx/html

COPY <<'EOF' /etc/nginx/conf.d/portal.conf
server {
    listen 80;
    server_name _;

    # Eviter "Request Header Or Cookie Too Large"
    large_client_header_buffers 4 32k;

    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ $uri.html /200.html;
    }

    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml text/javascript image/svg+xml;
    gzip_min_length 256;
}
EOF

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://127.0.0.1:80/ || exit 1
