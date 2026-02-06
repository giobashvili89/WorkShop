# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy all files
COPY . .

# Install dependencies
# Note: In restricted environments, you may need to build with --network=host
RUN npm install --prefer-offline --no-audit

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Remove default nginx static resources
RUN rm -rf ./*

# Copy built application from build stage
COPY --from=build /app/dist .

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
