Param (
    [ValidateSet("start", "stop")]
    [Parameter(Mandatory=$true)]
    [string] $cmd,
    [Parameter(Mandatory=$false)]
    [string] $env_file = ""
)

$env_file_command = "";

if ($env_file) {
  $env_file_command = "--env-file $env_file"
}

if ($cmd -eq "start") {
  Invoke-Expression "docker-compose -f docker-compose.dev-env.yml $env_file_command up -d"
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform init"
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform apply -input=false -auto-approve"
}
else {
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform destroy -input=false -auto-approve"
  Invoke-Expression "docker-compose -f docker-compose.dev-env.yml $env_file_command down -v --rmi local --remove-orphans"
}