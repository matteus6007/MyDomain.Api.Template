variable "default_secret" {
    default = {
        MyDomain = {
            Database = {
                User = "root"
                Password = "password123"
            }
        }
    }
}

resource "aws_secretsmanager_secret" "mydomain-secret" {
    name = "mydomain-api"
    description = "MyDomain API secret"
}

resource "aws_secretsmanager_secret_version" "mydomain-secret-version" {
    secret_id = aws_secretsmanager_secret.mydomain-secret.id
    secret_string = jsonencode(var.default_secret)
}
