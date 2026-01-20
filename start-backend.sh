#!/bin/bash
# Bash script to start both backend services
# Usage: ./start-backend.sh

echo "Starting AuditFlow Backend Services..."

# Start Simulator in background
cd AuditFlow.Simulator
dotnet run &
SIMULATOR_PID=$!

# Wait a bit
sleep 2

# Start Engine in background
cd ../AuditFlow.Engine
dotnet run &
ENGINE_PID=$!

cd ..

echo "Backend services started."
echo "Simulator (PID: $SIMULATOR_PID): http://localhost:5001"
echo "Engine (PID: $ENGINE_PID): http://localhost:5002"
echo ""
echo "To stop services, run: kill $SIMULATOR_PID $ENGINE_PID"
echo ""
echo "To start the frontend, run:"
echo "  cd AuditFlow.UI"
echo "  npm run dev"
