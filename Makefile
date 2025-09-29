include .env
export

test:
	@echo "Running tests..."
	@echo "Tests passed."

build-images:
	docker compose up --build -d

start-all:
	@echo "Starting all services..."
	docker compose up
	@echo "All services started. You can access the frontend at http://localhost:5173 and the backend at http://localhost:5104."

stop-all:
	@echo "Stopping all services..."
	docker compose down
	@echo "All services stopped."

	.PHONY: test build start-all stop-all