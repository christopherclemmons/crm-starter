variable "name_prefix" {
  description = "Standard naming prefix used for database resources."
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
  description = "Common tags applied to database resources."
  type        = map(string)
  default     = {}
}
