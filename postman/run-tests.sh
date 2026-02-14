#!/usr/bin/env bash
# ──────────────────────────────────────────────────────────
# run-tests.sh — Run PFM API Postman tests with Newman
# Generates an HTML report in  postman/reports/
# ──────────────────────────────────────────────────────────
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPORT_DIR="$SCRIPT_DIR/reports"

mkdir -p "$REPORT_DIR"

# Check if newman is installed
if ! command -v newman &>/dev/null; then
  echo "⚠️  Newman is not installed. Installing via npm..."
  npm install -g newman
fi

# Check if HTML reporter is installed
if ! npm list -g newman-reporter-htmlextra &>/dev/null 2>&1; then
  echo "⚠️  newman-reporter-htmlextra is not installed. Installing..."
  npm install -g newman-reporter-htmlextra
fi

TIMESTAMP=$(date +%Y%m%d-%H%M%S)
REPORT_FILE="$REPORT_DIR/pfm-api-test-report-${TIMESTAMP}.html"

echo "═══════════════════════════════════════════════════"
echo "  PFM API Tests — Newman Runner"
echo "═══════════════════════════════════════════════════"
echo ""
echo "  Collection : pfm-api-tests.postman_collection.json"
echo "  Environment: pfm-environment.json"
echo "  Report     : $REPORT_FILE"
echo ""

newman run "$SCRIPT_DIR/pfm-api-tests.postman_collection.json" \
  --environment "$SCRIPT_DIR/pfm-environment.json" \
  --working-dir "$SCRIPT_DIR" \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export "$REPORT_FILE" \
  --reporter-htmlextra-title "PFM API Test Report" \
  --reporter-htmlextra-browserTitle "PFM API Tests" \
  --color on \
  --delay-request 100

EXIT_CODE=$?

echo ""
if [ $EXIT_CODE -eq 0 ]; then
  echo "✅  All tests passed!"
else
  echo "❌  Some tests failed (exit code: $EXIT_CODE)"
fi

echo "📄  HTML report: $REPORT_FILE"
echo ""

# Open report in default browser (cross-platform)
if command -v xdg-open &>/dev/null; then
  xdg-open "$REPORT_FILE"  # Linux
elif command -v open &>/dev/null; then
  open "$REPORT_FILE"      # macOS
elif command -v start &>/dev/null; then
  start "$REPORT_FILE"     # Windows
else
  echo "ℹ️  Open the report manually: $REPORT_FILE"
fi

exit $EXIT_CODE
