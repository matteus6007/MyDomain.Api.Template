Param (
    [ValidateSet("start", "stop")]
    [Parameter(Mandatory=$true)]
    [string] $cmd,
    [Parameter(Mandatory=$false)]
    [string] $env_file = "",
    [Parameter(Mandatory=$false)]
    [boolean] $detach = $true
)

# Override the environment file
$env_file_command = "";

if ($env_file) {
  $env_file_command = "--env-file $env_file"
}

# Run containers in the background
$detach_command = "";

if ($detach) {
  $detach_command = "-d"
}

if ($cmd -eq "start") {
  Invoke-Expression "docker-compose -f docker-compose.dev-env.yml $env_file_command up $detach_command"
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform init"
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform apply -input=false -auto-approve"
}
else {
  Invoke-Expression "docker-compose -f docker-compose.dev-terraform.yml $env_file_command run --rm terraform destroy -input=false -auto-approve"
  Invoke-Expression "docker-compose -f docker-compose.dev-env.yml $env_file_command down -v --rmi local --remove-orphans"
}