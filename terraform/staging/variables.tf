variable "app_name" {
  description = "Application or service name."
  type        = string
  default     = "crm"
}

variable "owner" {
  description = "Team or system owner tag."
  type        = string
  default     = "platform"
}

variable "cost_center" {
  description = "Cost allocation tag."
  type        = string
  default     = "engineering"
}

variable "region" {
  description = "Target deployment region."
  type        = string
  default     = "us-east-1"
}

variable "extra_tags" {
  description = "Additional environment-specific tags."
  type        = map(string)
  default     = {}
}
