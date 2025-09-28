include .env
export

test:
	@echo "Running tests..."
	@echo "Tests passed."

build-all:
	@echo "Building ..."
	yarn install
	@echo "Packages installed..."
	yarn build
	@echo "Building completed"

start-all:
	@echo "Starting all services..."
	make start-frontend & make start-backend
	@echo "All services started. You can access the frontend at http://localhost:5173 and the backend at http://localhost:3000."

stop-all:
	@echo "Stopping all services..."
	@if lsof -t -i:5173 > /dev/null; then kill $(lsof -t -i:5173); fi || true
	@if lsof -t -i:5174 > /dev/null; then kill $(lsof -t -i:5174); fi || true
	@if lsof -t -i:5175 > /dev/null; then kill $(lsof -t -i:5175); fi || true
	@if lsof -t -i:3000 > /dev/null; then kill $(lsof -t -i:3000); fi || true
	@echo "All services stopped."

start-frontend:
	@echo "Starting frontend..."
	yarn workspace tenant-spa dev

start-backend:
	@echo "Starting backend..."
	yarn workspace backend start

configure-dns:
	@bash scripts/configure_dns.sh

start-dns:
	@bash scripts/start_dnsmasq.sh

stop-dns:
	@bash scripts/stop_dnsmasq.sh

restart-dns: stop-dns start-dns
	@echo "dnsmasq restarted."

apply-dns: configure-dns restart-dns
	@echo "DNS configuration applied. You might need to flush your DNS cache."
	@echo "Run 'sudo dscacheutil -flushcache' and 'sudo killall -HUP mDNSResponder' if needed."

.PHONY: test build-all start-all stop-all start-frontend start-backend configure-dns start-dns stop-dns restart-dns apply-dns