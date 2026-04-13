output "name_prefix" {
  description = "Standard production naming prefix."
  value       = local.name_prefix
}

output "common_tags" {
  description = "Shared production tags."
  value       = local.common_tags
}

output "network_module_name" {
  description = "Production network module naming output."
  value       = module.network.module_name
}

output "database_module_name" {
  description = "Production database module naming output."
  value       = module.database.module_name
}

output "compute_module_name" {
  description = "Production compute module naming output."
  value       = module.compute.module_name
}
