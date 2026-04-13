variable "name_prefix" {
  description = "Standard naming prefix used for compute resources."
  type        = string
}

variable "environment" {
  description = "Deployment environment name."
  type        = string
}

variable "region" {
  description = "Target deployment region."
  type        = string
}

variable "tags" {
  description = "Common tags applied to compute resources."
  type        = map(string)
  default     = {}
}
