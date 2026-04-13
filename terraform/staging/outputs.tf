output "name_prefix" {
  description = "Standard staging naming prefix."
  value       = local.name_prefix
}

output "common_tags" {
  description = "Shared staging tags."
  value       = local.common_tags
}

output "network_module_name" {
  description = "Staging network module naming output."
  value       = module.network.module_name
}

output "database_module_name" {
  description = "Staging database module naming output."
  value       = module.database.module_name
}

output "compute_module_name" {
  description = "Staging compute module naming output."
  value       = module.compute.module_name
}
