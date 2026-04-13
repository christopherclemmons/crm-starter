# Terraform Layout

This directory separates environment entrypoints from reusable service modules.

## Structure

- `modules/network`: networking resources such as VPCs, subnets, and security groups
- `modules/database`: data services such as PostgreSQL, subnet groups, and parameters
- `modules/compute`: application runtime resources such as ECS, app services, or instances
- `dev`: development environment entrypoint
- `staging`: staging environment entrypoint
- `prod`: production environment entrypoint

Each environment is isolated and should use its own backend configuration and variable values.

## Next Steps

1. Add provider and backend configuration for your target cloud.
2. Implement each service module with the cloud resources you need.
3. Keep environment-specific values in each environment directory.
