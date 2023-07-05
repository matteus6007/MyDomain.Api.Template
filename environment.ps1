Param (
    [ValidateSet("start", "stop")]
    [Parameter(Mandatory=$true)]
    [string] $cmd
)

if ($cmd -eq "start") {
  docker-compose -f docker-compose.dev-env.yml up -d
  docker run --rm -v ${pwd}/terraform:/terraform -w /terraform hashicorp/terraform -chdir=/terraform/local init
  docker run --rm --network mydomain-api -v ${pwd}/terraform:/terraform -w /terraform hashicorp/terraform -chdir=/terraform/local apply -input=false -auto-approve
}
else {
  docker run --rm --network mydomain-api -v ${pwd}/terraform:/terraform -w /terraform hashicorp/terraform -chdir=/terraform/local destroy -input=false -auto-approve
  docker-compose -f docker-compose.dev-env.yml down -v --rmi local --remove-orphans
}