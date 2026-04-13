output "name_prefix" {
  description = "Standard development naming prefix."
  value       = local.name_prefix
}

output "common_tags" {
  description = "Shared development tags."
  value       = local.common_tags
}

output "network_module_name" {
  description = "Development network module naming output."
  value       = module.network.module_name
}

output "database_module_name" {
  description = "Development database module naming output."
  value       = module.database.module_name
}

output "compute_module_name" {
  description = "Development compute module naming output."
  value       = module.compute.module_name
}
