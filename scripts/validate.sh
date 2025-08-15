#!/bin/bash

# Stickler Local Validation Script
# Runs the same checks as CI to catch issues before pushing

set -euo pipefail  # Exit on error, undefined vars, pipe failures

# Colors for output
readonly RED='\033[0;31m'
readonly GREEN='\033[0;32m'
readonly YELLOW='\033[1;33m'
readonly CYAN='\033[0;36m'
readonly NC='\033[0m' # No Color

# Script options
SKIP_TESTS=false
SKIP_BENCHMARKS=false
SKIP_FORMAT=false
FAST=false
VERBOSE=false

# Helper functions
log_info() {
    echo -e "${CYAN}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

log_step() {
    echo -e "${YELLOW}🔧 $1${NC}"
}

show_help() {
    cat << EOF
Stickler Local Validation Script

USAGE:
    ./scripts/validate.sh [OPTIONS]

OPTIONS:
    --skip-tests        Skip unit and integration tests
    --skip-benchmarks   Skip performance benchmarks
    --skip-format       Skip code formatting check
    --fast              Skip benchmarks and use faster test settings
    --verbose           Show detailed output
    --help              Show this help message

EXAMPLES:
    ./scripts/validate.sh                    # Full validation
    ./scripts/validate.sh --fast             # Quick validation
    ./scripts/validate.sh --skip-benchmarks  # Tests only
    ./scripts/validate.sh --verbose          # Detailed output

This script runs the same checks as the CI pipeline to catch issues
before pushing to GitHub.
EOF
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --skip-benchmarks)
            SKIP_BENCHMARKS=true
            shift
            ;;
        --skip-format)
            SKIP_FORMAT=true
            shift
            ;;
        --fast)
            FAST=true
            SKIP_BENCHMARKS=true
            shift
            ;;
        --verbose)
            VERBOSE=true
            shift
            ;;
        --help)
            show_help
            exit 0
            ;;
        *)
            log_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Verbose output setup
if [[ "$VERBOSE" == "true" ]]; then
    DOTNET_VERBOSITY="normal"
else
    DOTNET_VERBOSITY="minimal"
fi

# Check prerequisites
check_prerequisites() {
    log_step "Checking prerequisites..."

    if ! command -v dotnet &> /dev/null; then
        log_error ".NET SDK not found. Please install .NET 8 SDK."
        exit 1
    fi

    local dotnet_version
    dotnet_version=$(dotnet --version)
    if [[ ! "$dotnet_version" =~ ^8\. ]]; then
        log_warning "Expected .NET 8, found: $dotnet_version"
    fi

    if [[ ! -f "Stickler.sln" ]]; then
        log_error "Run this script from the repository root directory"
        exit 1
    fi

    log_success "Prerequisites OK (SDK: $dotnet_version)"
}

# Clean previous build artifacts
clean_build() {
    log_step "Cleaning previous build artifacts..."

    if [[ "$VERBOSE" == "true" ]]; then
        dotnet clean --verbosity "$DOTNET_VERBOSITY"
    else
        dotnet clean > /dev/null 2>&1
    fi

    # Clean test results
    find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name "BenchmarkDotNet.Artifacts" -type d -exec rm -rf {} + 2>/dev/null || true

    log_success "Clean completed"
}

# Restore dependencies
restore_dependencies() {
    log_step "Restoring NuGet packages..."

    if dotnet restore --verbosity "$DOTNET_VERBOSITY"; then
        log_success "Dependencies restored"
    else
        log_error "Failed to restore dependencies"
        exit 1
    fi
}

# Build solution
build_solution() {
    log_step "Building solution in Release configuration..."

    if dotnet build --configuration Release --no-restore --verbosity "$DOTNET_VERBOSITY"; then
        log_success "Build completed successfully"
    else
        log_error "Build failed"
        exit 1
    fi
}

# Check code formatting
check_formatting() {
    if [[ "$SKIP_FORMAT" == "true" ]]; then
        log_info "Skipping code formatting check"
        return 0
    fi

    log_step "Checking code formatting..."

    if dotnet format --verify-no-changes --verbosity diagnostic; then
        log_success "Code formatting is correct"
    else
        log_error "Code formatting issues found"
        log_info "Run 'dotnet format' to fix formatting issues"
        exit 1
    fi
}

# Run unit tests
run_unit_tests() {
    log_step "Running unit tests..."

    local test_args="--configuration Release --no-build --logger trx"
    if [[ "$VERBOSE" == "true" ]]; then
        test_args="$test_args --verbosity $DOTNET_VERBOSITY"
    else
        test_args="$test_args --verbosity quiet"
    fi

    if dotnet test tests/Stickler.Unit $test_args; then
        log_success "Unit tests passed"
    else
        log_error "Unit tests failed"
        exit 1
    fi
}

# Run integration tests
run_integration_tests() {
    log_step "Running integration tests..."

    local test_args="--configuration Release --no-build --logger trx"
    if [[ "$VERBOSE" == "true" ]]; then
        test_args="$test_args --verbosity $DOTNET_VERBOSITY"
    else
        test_args="$test_args --verbosity quiet"
    fi

    if dotnet test tests/Stickler.Integration $test_args; then
        log_success "Integration tests passed"
    else
        log_error "Integration tests failed"
        exit 1
    fi
}

# Run performance benchmarks
run_benchmarks() {
    if [[ "$SKIP_BENCHMARKS" == "true" ]]; then
        log_info "Skipping performance benchmarks"
        return 0
    fi

    log_step "Running performance benchmarks..."
    log_warning "This may take several minutes..."

    pushd tests/Stickler.Performance > /dev/null

    if dotnet run --configuration Release --exporters json; then
        log_success "Benchmarks completed"
    else
        log_error "Benchmarks failed"
        popd > /dev/null
        exit 1
    fi

    popd > /dev/null
}

# Validate package creation
validate_package() {
    log_step "Validating NuGet package creation..."

    if dotnet pack src/Stickler/Stickler.csproj --configuration Release --no-build --output ./temp-packages --verbosity "$DOTNET_VERBOSITY"; then
        # Clean up temp packages
        rm -rf ./temp-packages
        log_success "Package validation passed"
    else
        log_error "Package creation failed"
        rm -rf ./temp-packages
        exit 1
    fi
}

# Main execution
main() {
    local start_time
    start_time=$(date +%s)

    echo -e "${CYAN}"
    echo "🚀 Stickler Local Validation Pipeline"
    echo "=================================="
    echo -e "${NC}"

    # Run validation steps
    check_prerequisites
    clean_build
    restore_dependencies
    build_solution
    check_formatting

    if [[ "$SKIP_TESTS" == "false" ]]; then
        run_unit_tests
        run_integration_tests
    else
        log_info "Skipping tests"
    fi

    run_benchmarks
    validate_package

    # Calculate execution time
    local end_time duration
    end_time=$(date +%s)
    duration=$((end_time - start_time))

    echo -e "${GREEN}"
    echo "🎉 All validation checks passed!"
    echo "Execution time: ${duration}s"
    echo "Your code is ready to push! 🚀"
    echo -e "${NC}"
}

# Error handling
trap 'log_error "Validation failed at line $LINENO. Exit code: $?"' ERR

# Run main function
main "$@"
