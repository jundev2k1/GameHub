#!/bin/bash

# Default
COVERAGE=false
REPORT=false

# Validate the input parameters
for arg in "$@"
do
  if [ "$arg" == "--coverage" ]; then
    COVERAGE=true
  elif [ "$arg" == "--report" ]; then
    REPORT=true
  fi
done

# 1. Only test
if [ "$COVERAGE" = false ] && [ "$REPORT" = false ]; then
  echo "▶️  Running tests without coverage..."
  dotnet test
  exit 0
fi

# 2. Test coverage
if [ "$COVERAGE" = true ]; then
  echo "🧪 Running tests and collecting coverage..."

  # Run tests and collect data
  dotnet test --collect:"XPlat Code Coverage"

  # Find the latest coverage file
  LATEST_COV_FILE=$(find ./TestResults -type f -name "coverage.cobertura.xml" -print0 | xargs -0 ls -t | head -n1)

  if [ ! -f "$LATEST_COV_FILE" ]; then
    echo "❌ The coverage.cobertura.xml file was not found"
    exit 1
  fi

  # Rename the directory with the current time
  TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")
  RENAMED_DIR="./TestResults/$TIMESTAMP"
  mkdir -p "$RENAMED_DIR"
  cp "$LATEST_COV_FILE" "$RENAMED_DIR/coverage.cobertura.xml"
  echo "✅ Coverage saved to: $RENAMED_DIR"

  # Copy to the standard location
  mkdir -p TestResults/coverage
  cp "$RENAMED_DIR/coverage.cobertura.xml" TestResults/coverage/coverage.cobertura.xml
fi

# 3. Generate an HTML report
if [ "$COVERAGE" = true ] || [ "$REPORT" = true ]; then
  echo "📊 Generating HTML report..."

    # Clean Coverage Report
    if [ -d "CoverageReport" ]; then
      rm -rf CoverageReport
    fi

  # Generate coverage report
  dotnet tool run reportgenerator \
    -reports:TestResults/coverage/coverage.cobertura.xml \
    -targetdir:CoverageReport \
    -reporttypes:HtmlInline_AzurePipelines

  if [ -f "CoverageReport/index.html" ]; then
    echo "✅ HTML report available at:CoverageReport/index.html"
    
    # 🆕 Auto open file
    open CoverageReport/index.html
  else
    echo "❌ Report generation failed!"
  fi
fi
