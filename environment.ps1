Param (
    [ValidateSet("start", "stop")]
    [Parameter(Mandatory=$true)]
    [string] $cmd
)

if ($cmd -eq "start") {
  docker-compose -f docker-compose.dev-env.yml up -d
}
else {
  docker-compose -f docker-compose.dev-env.yml down -v --rmi local --remove-orphans
}