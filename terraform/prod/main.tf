locals {
  environment = "prod"
  name_prefix = "${var.app_name}-${local.environment}"
  common_tags = merge(
    {
      Environment = local.environment
      Service     = var.app_name
      Owner       = var.owner
      CostCenter  = var.cost_center
      Region      = var.region
      ManagedBy   = "terraform"
    },
    var.extra_tags
  )
}

module "network" {
  source = "../modules/network"

  name_prefix = local.name_prefix
  environment = local.environment
  region      = var.region
  tags        = local.common_tags
}

module "database" {
  source = "../modules/database"

  name_prefix = local.name_prefix
  environment = local.environment
  region      = var.region
  tags        = local.common_tags
}

module "compute" {
  source = "../modules/compute"

  name_prefix = local.name_prefix
  environment = local.environment
  region      = var.region
  tags        = local.common_tags
}
