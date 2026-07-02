#!/usr/bin/env bash
# Pulls the latest code, republishes, and restarts the MoneyBall service.
# Run this ON THE NUC from inside the cloned repo (/opt/moneyball/src).
set -euo pipefail

REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PUBLISH_DIR="/opt/moneyball/app"

cd "$REPO_DIR"
git pull
dotnet publish MoneyBall/MoneyBall.csproj -c Release -o "$PUBLISH_DIR"
sudo systemctl restart moneyball
sudo systemctl status moneyball --no-pager
