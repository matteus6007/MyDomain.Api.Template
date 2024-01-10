terraform {
  backend "local" {}
  required_providers {
    aws = {
        source = "hashicorp/aws"
    }
  }  
}

provider "aws" {
  access_key                  = "mock_access_key"
  region                      = "eu-west-1"
  s3_use_path_style           = true
  secret_key                  = "mock_secret_key"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    s3              = "http://localstack:4566"
    secretsmanager  = "http://localstack:4566"
    sns             = "http://localstack:4566"
    sqs             = "http://localstack:4566"
    sts             = "http://localstack:4566"
  }
}
