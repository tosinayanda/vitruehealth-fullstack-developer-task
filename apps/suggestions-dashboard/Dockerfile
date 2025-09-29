# Stage 1: Build the application
FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
# Pass build-time environment variables
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:1.27-alpine AS production
# Copy built assets from the 'build' stage
COPY --from=build /app/dist /usr/share/nginx/html
# Copy the Nginx configuration file
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]